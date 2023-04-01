using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using indexT = System.Int32;
using static SouvlakMVP.Graph;

namespace SouvlakMVP;

public class Euler
{
    /// <summary>Finds an Eulerian cycle in the given undirected graph using Hierholzer's algorithm.</summary>
    /// <param name="graph">The graph to search.</param>
    /// <param name="startVertexP">Optional starting vertex.</param>
    /// <returns>The Eulerian cycle as a list of vertices.</returns>
    public static List<indexT> FindEulerCycle(Graph graph, indexT? startVertexP = null)
    {
        graph = graph.DeepCopy();
        indexT startVertex = 0;

        // Check the correctness of the input parameter
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
        // List for final path 
        List<indexT> eulerCycle = new List<indexT>();

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
                graph.RemoveEdge(tempVertexID, edge.targetIdx);
            }
        }

        return eulerCycle;
    }
}
