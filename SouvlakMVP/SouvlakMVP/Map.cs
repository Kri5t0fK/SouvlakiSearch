using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SouvlakMVP;

/// <summary>
/// Class for storing city's map (aka. graph in which vertices = intersections and edges = roads)
/// </summary>
internal class Map
{
    /// <summary>
    /// Struct for holding data of a single intersection.
    /// Field "position" holds data used to display intersection on screen.
    /// Field "connections: stores all connections in form of tuples (targetID, distance).
    /// </summary>
    public struct Intersection
    {
        public Vector2 position;
        public List<(int targetID, int distance)> connections;


        public Intersection(Vector2 position, List<(int targetID, int distance)> connections)
        {
            this.position = position;
            this.connections = connections;
        }

        public Intersection(Vector2 position)
        {
            this.position = position;
            this.connections = new List<(int targetID, int distance)>();
        }

        public override string ToString()
        {
            string str = "(" + this.position.ToString() + ") : <";
            for (int i = 0; i < this.connections.Count - 1; i++)
            {
                str += this.connections[i].targetID.ToString() + ":" + this.connections[i].distance.ToString() + ", ";
            }
            str += this.connections[this.connections.Count].targetID.ToString() + ":" + this.connections[this.connections.Count].distance.ToString() + ">";
            return str;
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is Intersection)
            {
                Intersection other = (Intersection)obj;
                return this.position == other.position;
            }
            else
            { 
                return false; 
            }
        }

        public override int GetHashCode()
        {
            return this.position.GetHashCode();
        }

        public static bool operator ==(Intersection i1, Intersection i2) => i1.position == i2.position;
        public static bool operator !=(Intersection i1, Intersection i2) => i1.position != i2.position;

        public (int targetID, int distance) this[int id]
        {
            get { return this.connections[id];  }
            set { this.connections[id] = value; }
        } 
    }


    private List<Intersection> graph;


    public Map(List<Intersection> graph)
    {
        this.graph = graph;
    }

    public Map(Intersection intersection)
    {
        this.graph = new List<Intersection>() { intersection };
    }

    public Map()
    {
        this.graph = new List<Intersection>();
    }

    public override string ToString()
    {
        string str = "";
        foreach (Intersection intersection in this.graph)
        {
            str += intersection.ToString() + "\n";
        }
        return str;
    }

    public void AddIntersection(Intersection intersection)
    {
        // Check if intersection exists
        int thisID = this.graph.IndexOf(intersection);
        if (thisID < 0)
        {
            // If not add it at the end of list
            thisID = this.graph.Count();      
        }
        else
        {
            // If it does clear current connections, since we cant have duplicate intersections
            foreach(Intersection inter in this.graph)
            {
                inter.connections.RemoveAll(connection => connection.targetID == thisID);
            }

        }
        this.graph[thisID] = intersection;
        foreach ((int targetID, int distance) connection in intersection.connections)
        {
            this.graph[connection.targetID].connections.Add((thisID, connection.distance));
        }
    }

    public void AddConnection(int firstID, int secondID, int distance) 
    {
        // No checking for duplicates, we can have multiple connections between two intersections
        this.graph[firstID].connections.Add((secondID, distance));
        this.graph[secondID].connections.Add((firstID, distance));
    }

    public void RemoveIntersection(Intersection intersection)
    {
        throw new NotImplementedException();
    }

    public void RemoveIntersection(Vector2 position)
    {
        throw new NotImplementedException();
    }

    public void RemoveConnection(int firstID, int secondID)
    {
        this.graph[firstID].connections.RemoveAll(connection => connection.targetID == secondID);
        this.graph[secondID].connections.RemoveAll(connection => connection.targetID == firstID);
    }

    public Intersection this[int i]
    {
        get { return this.graph[i]; }
        set { this.AddIntersection(value); }
    }

    public int this[int i, int j] 
    { 
        get { return this.graph[i].connections[j].distance; }
        set { this.AddConnection(i, j, value); }
    }   
}
