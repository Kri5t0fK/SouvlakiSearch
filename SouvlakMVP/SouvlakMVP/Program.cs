﻿// See https://aka.ms/new-console-template for more information
using System;
using System.Numerics;

using indexT = System.Int32;
using edgeWeightT = System.Single;

namespace SouvlakMVP;

class Program
{
    static void Main(string[] args)
    {
        // Example map (graph) with 6 intersections (vertices) 
        Graph graph = new Graph();
        // Intersections from 0 to 5 
        graph.AddVertex(new Graph.Vertex(new Vector2(0, 0)));
        graph.AddVertex(new Graph.Vertex(new Vector2(1, 0)));
        graph.AddVertex(new Graph.Vertex(new Vector2(0, 1)));
        graph.AddVertex(new Graph.Vertex(new Vector2(1, 1)));
        graph.AddVertex(new Graph.Vertex(new Vector2(2, 1)));
        graph.AddVertex(new Graph.Vertex(new Vector2(1, 2)));
        // 9 roads and their distances 
        graph.AddEdge(0, 1, 3f);
        graph.AddEdge(0, 5, 6f);
        graph.AddEdge(0, 4, 3f);
        graph.AddEdge(1, 2, 1f);
        graph.AddEdge(1, 3, 3f);
        graph.AddEdge(2, 3, 3f);
        graph.AddEdge(2, 5, 1f);
        graph.AddEdge(3, 5, 1f);
        graph.AddEdge(4, 5, 2f);


        // Map visualization
        Console.WriteLine(graph.ToString());

        VerticesConnections vercon = new VerticesConnections(graph);
        Console.WriteLine(vercon.ToString());
        
        var con = vercon[0, 2];
        var p1 = con[1];
        var con2 = vercon[0, 3];
        var con3 = vercon[2, 3];
        Console.WriteLine("\n\n");

        Console.WriteLine(vercon.ToString());

        // Method1: Giving start and end vertex -> one path
        /*      
        indexT startstartVertex = 0;
        indexT endstartVertex = 5;
        (List<indexT>, edgeWeightT) result = Dijkstra.FindShortestPath(graph, startstartVertex, endstartVertex);
        Console.WriteLine("Order of intersections: " + String.Join(" -> ", result.Item1)); ;
        Console.WriteLine("Minimal cost: " + result.Item2);
        */

        // Method2: Giving only start vertex -> all paths from start vertex 
        /*
        indexT startVertex = 0;
        Dictionary<indexT, (List<indexT>, edgeWeightT)> results = Dijkstra.FindShortestPath(graph, startVertex);
        foreach (var result in results)
        {
            Console.WriteLine("\nResult for (" + startVertex + ", " + result.Key + "):");
            Console.WriteLine("Order of intersections: " + String.Join(" -> ", result.Value.Item1));
            Console.WriteLine("Minimal cost: " + result.Value.Item2);
        }
        */

        // Method2: Giving list of vertices -> Paths and costs for every combination of vertices pairs 
        /*
        List<indexT> vertices = new List<indexT> { 0, 1, 2, 3, 4, 5};
        Dictionary<HashSet<indexT>, (List<indexT>, edgeWeightT)> results = Dijkstra.FindShortestPath(graph, vertices);
        foreach (var result in results)
        {
            List<indexT> pair = result.Key.ToList();
            (List<indexT>, edgeWeightT) pathAndCost = result.Value;
            Console.WriteLine("\nResult for (" + pair[0] + ", " + pair[1] + "):");
            Console.WriteLine("Order of intersections: " + String.Join(" -> ", pathAndCost.Item1));
            Console.WriteLine("Minimal cost: " + pathAndCost.Item2);
        }
        */
    }
}
