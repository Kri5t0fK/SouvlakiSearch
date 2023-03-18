// See https://aka.ms/new-console-template for more information
using System.Numerics;

namespace SouvlakMVP;

class Program
{
    static void Main(string[] args)
    {
        Map map= new Map();
        map.AddIntersection(new Map.Intersection(new Vector2(0, 0)));
        map.AddIntersection(new Vector2(1, 0), new Map.Road[1] { new Map.Road(0, 1f) });
        map.AddIntersection(new Vector2(1, 1), new Map.Road[2] { new Map.Road(0, 1.44f), new Map.Road(1, 1f) });
        Console.WriteLine(map.ToString());
    }
}
