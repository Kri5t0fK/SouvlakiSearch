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
        Map map = new Map();
        // Intersections from 0 to 5 
        map.AddIntersection(new Map.Intersection(new Vector2(0, 0)));
        map.AddIntersection(new Map.Intersection(new Vector2(1, 0)));
        map.AddIntersection(new Map.Intersection(new Vector2(0, 1)));
        map.AddIntersection(new Map.Intersection(new Vector2(1, 1)));
        map.AddIntersection(new Map.Intersection(new Vector2(2, 1)));
        map.AddIntersection(new Map.Intersection(new Vector2(1, 2)));
        // 9 roads and their distances 
        map.AddRoad(0, 1, 3f);
        map.AddRoad(0, 5, 6f);
        map.AddRoad(0, 4, 3f);
        map.AddRoad(1, 2, 1f);
        map.AddRoad(1, 3, 3f);
        map.AddRoad(2, 3, 3f);
        map.AddRoad(2, 5, 1f);
        map.AddRoad(3, 5, 1f);
        map.AddRoad(4, 5, 2f);

        // Map visualization
        Console.WriteLine(map.ToString());

        indexT startIntersection = 0;
        indexT endIntersection = 5;
        (List<indexT>, distanceT) result = Dijkstra.FindShortestPath(map, startIntersection, endIntersection);
        // Co właściwie powinna zwracać metoda? Obecnie zwraca zwykłą listę indexT oraz koszt distanceT

        Console.WriteLine("Order of intersections: " + String.Join(" -> ", result.Item1)); ;
        Console.WriteLine("Minimal cost: " + result.Item2);
        
    }
}
