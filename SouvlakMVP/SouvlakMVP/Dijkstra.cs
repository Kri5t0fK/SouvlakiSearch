using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using indexT = System.Int32;
using distanceT = System.Single;
using System.Runtime.InteropServices;

namespace SouvlakMVP;

public class Dijkstra
{
    public static (List<indexT>, distanceT) FindShortestPath(Graph graph, indexT startVertex, indexT endVertex)
    {
        int verticesN = graph.GetVertexCount();

        // Array containing the minimum costs to reach each vertex from the starting vertex
        distanceT[] minCostToVertex = new distanceT[verticesN];
        // Array containing the preceding vertices on the path from the starting vertex
        indexT?[] precedingVertices = new indexT?[verticesN];

        // Working lists of vertices
        List<indexT> verticesToProcess = new List<indexT>();
        List<indexT> processedVertices = new List<indexT>();

        // Filing out verticesToProcess and minCostToVertex
        for (int i = 0; i < verticesN; i++)
        {
            minCostToVertex[i] = (i == startVertex) ? 0f : distanceT.MaxValue;
            verticesToProcess.Add(i);
        }

        while (verticesToProcess.Count > 0)
        {
            indexT processedVertex = indexT.MaxValue;
            distanceT tempMinCost = distanceT.MaxValue;

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
                Graph.Edge edge= graph[processedVertex].edgeList[i];
                indexT nextVertex = edge.targetIdx;
                distanceT edgeCost = edge.weight;

                // Check if neighbour has not yet been processed
                if (verticesToProcess.Contains(nextVertex))
                {
                    distanceT CostToVertex = minCostToVertex[processedVertex] + edgeCost;

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
        distanceT totalCost = minCostToVertex[endVertex];
        List<indexT> shortestPathFromStart = Enumerable.Reverse(shortestPathFromEnd).ToList();

        return (shortestPathFromStart, totalCost);
    }
}
