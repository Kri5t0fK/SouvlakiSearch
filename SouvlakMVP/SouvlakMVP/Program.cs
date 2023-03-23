// See https://aka.ms/new-console-template for more information
using System;
using System.Numerics;

using indexT = System.Int32;
using distanceT = System.Single;

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

        
        indexT startIntersection = 0;
        indexT endIntersection = 5;
        (List<indexT>, distanceT) result = Dijkstra.FindShortestPath(graph, startIntersection, endIntersection);
        // Co właściwie powinna zwracać metoda? Obecnie zwraca zwykłą listę indexT oraz koszt distanceT

        Console.WriteLine("Order of intersections: " + String.Join(" -> ", result.Item1)); ;
        Console.WriteLine("Minimal cost: " + result.Item2);
        
        
    }
}
