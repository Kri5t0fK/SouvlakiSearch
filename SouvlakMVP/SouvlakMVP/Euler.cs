using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using indexT = System.Int32;
using edgeWeightT = System.Single;
using static SouvlakMVP.Graph;

namespace SouvlakMVP;

public class Euler
{
    /// <summary>Finds an Eulerian cycle in the given undirected graph using Hierholzer's algorithm.</summary>
    /// <param name="graph">The graph to search.</param>
    /// <param name="startVertexP">Optional starting vertex.</param>
    /// <returns>Tuple with the Eulerian cycle as a list of vertices and total cost.</returns>
    public static (List<indexT>, edgeWeightT) FindEulerCycle(Graph graph, indexT? startVertexP = null)
    {
        graph = graph.DeepCopy();
        indexT startVertex = 0;

        // Check the correctness of the input parameter
        if (!HasEulerCycle(graph))
        {
            throw new Exception("Graph does not have an euler cycle.");
        }

        if (startVertexP != null)
        {
            indexT tempID = startVertexP.Value;
            if (tempID >= 0 && tempID < graph.GetVertexCount())
            {
                startVertex = tempID;
            }
            else
            {
                throw new ArgumentException($"There is no vertex with ID = {tempID} in the graph.");
            }
        }

        // Stack of vertices with non-zero degree
        Stack<indexT> nonIsolatedVerticesID = new Stack<indexT>(new[] { startVertex });

        List<indexT> eulerCycle = new List<indexT>();
        edgeWeightT totalCost = 0;

        while (nonIsolatedVerticesID.Count > 0)
        {
            // Get the vertexID from top of the stack (without removing)
            indexT tempVertexID = nonIsolatedVerticesID.Peek();
            // Get the list of the edges coming out from the vertex
            List<Edge> edgesFromVertex = graph[tempVertexID].edgeList;

            if (edgesFromVertex.Count == 0)
            {
                // Add vertexID to final path and delate it from stack
                eulerCycle.Add(tempVertexID);
                nonIsolatedVerticesID.Pop();
            }
            else
            {
                // Find any edge coming out of vertex, add its target to stack and remove it from graph
                Edge edge = edgesFromVertex[0];
                nonIsolatedVerticesID.Push(edge.targetIdx);
                totalCost += edge.weight;
                graph.RemoveEdge(tempVertexID, edge.targetIdx);
            }
        }

        return (eulerCycle, totalCost);
    }

    /// <summary>Checks if the given undirected graph contains an Euler cycle.</summary>
    /// <param name="graph">The graph to search.</param>
    /// <returns>True if the graph contains an Euler cycle, false otherwise.</returns>
    public static bool HasEulerCycle(Graph graph)
    {
        int verticesN = graph.GetVertexCount();

        for (int i = 0; i < verticesN; i++)
        {
            int edgesN = graph[i].GetEdgeCount();
            if (edgesN == 0 || (edgesN % 2) != 0)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>Checks if the given undirected graph contains an Euler path.</summary>
    /// <param name="graph">The graph to search.</param>
    /// <returns>True if the graph contains an Euler path, false otherwise.</returns>
    public static bool HasEulerPath(Graph graph)
    {
        int verticesN = graph.GetVertexCount();
        int oddVerticesCounter = 0;

        for (int i = 0; i < verticesN; i++)
        {
            int edgesN = graph[i].GetEdgeCount();
            if (edgesN == 0)
            {
                return false;
            }
            else if ((edgesN % 2) != 0 && ++oddVerticesCounter > 2)
            {
                return false;
            }
        }
        return true;
    }
}
