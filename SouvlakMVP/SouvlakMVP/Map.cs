using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;


using indexT = System.Int32;
using distanceT = System.Single;    // no maidens?


namespace SouvlakMVP;


/// <summary>
/// Class for storing city's map (aka. graph in which vertices = intersections and edges = roads)
/// </summary>
internal class Map
{
    /// <summary>
    /// Struct for holding data of a single intersection.
    /// Field "position" holds data used to display intersection on screen.
    /// Field "connections" stores all connections in form of tuples (targetID, distance).
    /// </summary>
    public struct Intersection
    {
        public Vector2 position;
        public List<(indexT targetID, distanceT distance)> connections;


        public Intersection(Vector2 position, List<(indexT targetID, distanceT distance)> connections)
        {
            this.position = position;
            this.connections = connections;
        }

        public Intersection(Vector2 position)
        {
            this.position = position;
            this.connections = new List<(indexT targetID, distanceT distance)>();
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

        public (indexT targetID, distanceT distance) this[int id]
        {
            get { return this.connections[id];  }
            set { this.connections[id] = value; }
        } 
    }

    public class DuplicateIntersectionException : Exception
    {
        public DuplicateIntersectionException() : base("Duplicate intersections are not allowed!") { }
        public DuplicateIntersectionException(string message) : base(message) { }
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
        if (this.graph.Any(inter => inter.position == intersection.position))
        {
            throw new DuplicateIntersectionException();
        }
        else
        {
            indexT thisID = this.graph.Count();
            this.graph.Add(intersection);
            foreach ((indexT targetID, distanceT distance) connection in intersection.connections)
            {
                this.graph[connection.targetID].connections.Add((thisID, connection.distance));
            }
        }
    }

    public void AddIntersection(Vector2 position, List<(indexT targetID, distanceT distance)> connections)
    {
        this.AddIntersection(new Intersection(position, connections));
    }

    public void UpdateIntersection(Intersection intersection)
    {
        indexT thisID = this.graph.IndexOf(intersection);
        if (thisID < 0)
        {
            throw new IndexOutOfRangeException("Cannot update intersection that does not exist!");
        }
        else
        {
            foreach (Intersection inter in this.graph)
            {
                inter.connections.RemoveAll(connection => connection.targetID == thisID);
            }
            this.graph[thisID] = intersection;
            foreach ((int targetID, int distance) connection in intersection.connections)
            {
                this.graph[connection.targetID].connections.Add((thisID, connection.distance));
            }
        }
    }

    public void UpdateIntersection(Vector2 position, List<(indexT targetID, distanceT distance)> connections)
    {
        this.UpdateIntersection(new Intersection(position, connections));
    }

    public void RemoveIntersection(Vector2 position)
    {
        throw new NotImplementedException();
    }

    public void RemoveIntersection(Intersection intersection)
    {
        throw new NotImplementedException();
    }


    public void AddConnection(indexT firstID, indexT secondID, distanceT distance) 
    {
        // No checking for duplicates, we can have multiple connections between two intersections
        this.graph[firstID].connections.Add((secondID, distance));
        this.graph[secondID].connections.Add((firstID, distance));
    }

    public void UpdateConnection(indexT firstID, indexT secondID, distanceT distance)
    {
        throw new NotImplementedException();
    }

    public void RemoveConnection(indexT firstID, indexT secondID)
    {
        this.graph[firstID].connections.RemoveAll(connection => connection.targetID == secondID);
        this.graph[secondID].connections.RemoveAll(connection => connection.targetID == firstID);
    }
   
    public Intersection this[indexT i]
    {
        get { return this.graph[i]; }
        set { this.AddIntersection(value); }
    }

    public distanceT this[indexT i, indexT j] 
    { 
        get { return this.graph[i].connections[j].distance; }
        set { this.AddConnection(i, j, value); }
    }   
}
