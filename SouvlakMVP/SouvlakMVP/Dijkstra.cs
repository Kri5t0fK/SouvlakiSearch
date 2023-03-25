using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using indexT = System.Int32;
using edgeWeightT = System.Single;
using System.Runtime.InteropServices;

namespace SouvlakMVP;

public class Dijkstra
{
    /// <summary>Finds the shortest path between a starting vertex and final vertex in a graph using Dijkstra's algorithm.</summary>
    /// <param name="graph">The graph to search.</param>
    /// <param name="startVertex">The vertex to start the search from.</param>
    /// <param name="endVertex">The destination vertex of the search.</param>
    /// <returns>A tuple containing the shortest path and the distance.</returns>
    public static (List<indexT>, edgeWeightT) FindShortestPath(Graph graph, indexT startVertex, indexT endVertex)
    {
        int verticesN = graph.GetVertexCount();

        // Array containing the minimum costs to reach each vertex from the starting vertex
        edgeWeightT[] minCostToVertex = new edgeWeightT[verticesN];
        // Array containing the preceding vertices on the path from the starting vertex
        indexT?[] precedingVertices = new indexT?[verticesN];

        // Working lists of vertices
        List<indexT> verticesToProcess = new List<indexT>();
        List<indexT> processedVertices = new List<indexT>();

        // Filing out verticesToProcess and minCostToVertex
        for (int i = 0; i < verticesN; i++)
        {
            minCostToVertex[i] = (i == startVertex) ? 0f : edgeWeightT.MaxValue;
            verticesToProcess.Add(i);
        }

        while (verticesToProcess.Count > 0)
        {
            indexT processedVertex = indexT.MaxValue;
            edgeWeightT tempMinCost = edgeWeightT.MaxValue;

            // Finding the vertex to process (with the minimum cost to reach from the starting vertex)
            foreach (indexT vertex in verticesToProcess)
            {
                if (minCostToVertex[vertex] < tempMinCost)
                {
                    tempMinCost = minCostToVertex[vertex];
                    processedVertex = vertex;
                }
            }

            // Moving the current vertex to processed vertices
            if (verticesToProcess.Remove(processedVertex))
            {
                processedVertices.Add(processedVertex);
            }

            // Reviewing all neighbors of the relocated vertex
            for (indexT i = 0; i < graph[processedVertex].edgeList.Count(); i++)
            {
                Graph.Edge edge = graph[processedVertex].edgeList[i];
                indexT nextVertex = edge.targetIdx;
                edgeWeightT edgeCost = edge.weight;

                // Check if neighbour has not yet been processed
                if (verticesToProcess.Contains(nextVertex))
                {
                    edgeWeightT CostToVertex = minCostToVertex[processedVertex] + edgeCost;

                    // Check the new cost and update if it is smaller than the old one
                    if (minCostToVertex[nextVertex] > CostToVertex)
                    {
                        minCostToVertex[nextVertex] = CostToVertex;
                        precedingVertices[nextVertex] = processedVertex;
                    }
                }
            }
        }

        indexT? tempVertex = endVertex;
        List<indexT> shortestPathFromEnd = new List<indexT>();

        // Create the shortest path
        while (tempVertex != null)
        {
            shortestPathFromEnd.Add(tempVertex.Value);
            tempVertex = precedingVertices[tempVertex.Value];
        }

        // Prepare finall results 
        edgeWeightT totalCost = minCostToVertex[endVertex];
        List<indexT> shortestPathFromStart = Enumerable.Reverse(shortestPathFromEnd).ToList();

        return (shortestPathFromStart, totalCost);
    }

    /// <summary>Finds the shortest paths from a starting vertex to all other vertices in the graph using the Dijkstra algorithm.</summary>
    /// <param name="graph">The graph to search.</param>
    /// <param name="startVertex">The vertex to start the search from.</param>
    /// <returns>A list of tuples containing shortest path and the distance.</returns>
    public static Dictionary<indexT, (List<indexT>, edgeWeightT)> FindShortestPath(Graph graph, indexT startVertex)
    {
        int verticesN = graph.GetVertexCount();

        // Array containing the minimum costs to reach each vertex from the starting vertex
        edgeWeightT[] minCostToVertex = new edgeWeightT[verticesN];
        // Array containing the preceding vertices on the path from the starting vertex
        indexT?[] precedingVertices = new indexT?[verticesN];

        // Working lists of vertices
        List<indexT> verticesToProcess = new List<indexT>();
        List<indexT> processedVertices = new List<indexT>();

        // Filing out verticesToProcess and minCostToVertex
        for (int i = 0; i < verticesN; i++)
        {
            minCostToVertex[i] = (i == startVertex) ? 0f : edgeWeightT.MaxValue;
            verticesToProcess.Add(i);
        }

        while (verticesToProcess.Count > 0)
        {
            indexT processedVertex = indexT.MaxValue;
            edgeWeightT tempMinCost = edgeWeightT.MaxValue;

            // Finding the vertex to process (with the minimum cost to reach from the starting vertex)
            foreach (indexT vertex in verticesToProcess)
            {
                if (minCostToVertex[vertex] < tempMinCost)
                {
                    tempMinCost = minCostToVertex[vertex];
                    processedVertex = vertex;
                }
            }

            // Moving the current vertex to processed vertices
            if (verticesToProcess.Remove(processedVertex))
            {
                processedVertices.Add(processedVertex);
            }

            // Reviewing all neighbors of the relocated vertex
            for (indexT i = 0; i < graph[processedVertex].edgeList.Count(); i++)
            {
                Graph.Edge edge = graph[processedVertex].edgeList[i];
                indexT nextVertex = edge.targetIdx;
                edgeWeightT edgeCost = edge.weight;

                // Check if neighbour has not yet been processed
                if (verticesToProcess.Contains(nextVertex))
                {
                    edgeWeightT CostToVertex = minCostToVertex[processedVertex] + edgeCost;

                    // Check the new cost and update if it is smaller than the old one
                    if (minCostToVertex[nextVertex] > CostToVertex)
                    {
                        minCostToVertex[nextVertex] = CostToVertex;
                        precedingVertices[nextVertex] = processedVertex;
                    }
                }
            }
        }

        List<indexT> shortestPath = new List<indexT>();
        Dictionary<indexT, (List<indexT>, edgeWeightT)> pathsAndWeights = new Dictionary<indexT, (List<indexT>, edgeWeightT)>();

        processedVertices.Remove(startVertex);
        foreach (indexT endVertex in processedVertices) 
        {
            indexT? tempVertex = endVertex;

            // Create the shortest path
            while (tempVertex != null)
            {
                shortestPath.Add(tempVertex.Value);
                tempVertex = precedingVertices[tempVertex.Value];
            }

            edgeWeightT totalCost = minCostToVertex[endVertex];
            shortestPath.Reverse();
            pathsAndWeights.Add(endVertex, (shortestPath, totalCost));
            shortestPath = new List<indexT>();
        }

        return pathsAndWeights;
    }

    /// <summary>Finds the shortest paths between all combinations of vertices pairs (RoundRobin)</summary>
    /// <param name="graph">The graph to search.</param>
    /// <param name="vertices">The list of vertices.</param>
    /// <returns>A dictionary containing the shortest path and weight for every vertices pair.</returns>
    public static Dictionary<(indexT, indexT), (List<indexT>, edgeWeightT)> FindShortestPath(Graph graph, List<indexT> vertices)
    {
        List<(indexT, indexT)> verticesPairs = GetAllPairs(vertices);
        List<indexT> uniqueStartVertices = new List<indexT>();

        // Fill in the list of unique staring vertices
        foreach ((indexT, indexT) pair in verticesPairs) 
        {
            if (!uniqueStartVertices.Contains(pair.Item1))
            {
                uniqueStartVertices.Add(pair.Item1);
            }
        }

        Dictionary<(indexT, indexT), (List<indexT>, edgeWeightT)> result = new Dictionary<(indexT, indexT), (List<indexT>, edgeWeightT)>();

        // Calculate paths and costs for every unique verices
        foreach (indexT StartVertex in uniqueStartVertices)
        {
            Dictionary<indexT, (List<indexT>, edgeWeightT)> pathsAndCosts = FindShortestPath(graph, StartVertex);
            foreach ((indexT, indexT) pair in verticesPairs.Where(pair => pair.Item1 == StartVertex))
            {
                (List<indexT>, edgeWeightT) pathAndCost = pathsAndCosts[pair.Item2];
                result.Add(pair, pathAndCost);
            }
        }

        return result;
    }

    /// <summary>Calculates all combinations of vertices pairs.</summary>
    /// <param name="vertices">The list of vertices.</param>
    /// <returns>A list of tuples containing vertices pairs.</returns>
    private static List<(indexT, indexT)> GetAllPairs(List<indexT> vertices)
    {
        List<(indexT, indexT)> pairs = new List<(indexT, indexT)>();

        for (int i = 0; i < vertices.Count; i++)
        {
            for (int j = i + 1; j < vertices.Count; j++)
            {
                pairs.Add((vertices[i], vertices[j]));
            }
        }

        return pairs;
    }
}
