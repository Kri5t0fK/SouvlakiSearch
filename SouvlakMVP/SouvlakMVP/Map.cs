// Most of those can probably be removed
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualBasic;
using System.Numerics;
using System.ComponentModel;


using indexT = System.Int32;
using distanceT = System.Single;    // no maidens?

namespace SouvlakMVP;

/// <summary>
/// Class for storing city's map (aka. graph in which vertices = intersections and edges = roads)
/// </summary>
internal class Map
{
    /// <summary>
    /// Class responsible for holding data about single connection, aka. target's ID and distance 
    /// </summary>
    public struct Road
    {
        public indexT targetIdx;
        public distanceT distance;

        public Road(indexT targetIdx, distanceT distance)
        { 
            this.targetIdx = targetIdx;
            this.distance = distance; 
        }

        public override string ToString()
        {
            return this.targetIdx.ToString() + ": " + this.distance.ToString("n2");
        }

        public override bool Equals(object? obj)
        {
            if (obj != null && obj is Road)
            {
                Road other = (Road)obj;
                return this.targetIdx == other.targetIdx;
            }
            else
            {
                return false;
            };
        }

        public override int GetHashCode()
        {
            return this.targetIdx.GetHashCode() * 997 + this.distance.GetHashCode();    // just multiply one value by big prime number
        }

        public static bool operator ==(Road r1, Road r2) => r1.targetIdx == r2.targetIdx;
        public static bool operator !=(Road r1, Road r2) => r1.targetIdx != r2.targetIdx;
    }


    /// <summary>
    /// Struct for holding data of a single intersection, that is it's position and list of roads (connections)
    /// </summary>
    public struct Intersection
    {
        public Vector2 position;
        public List<Road> roads;

        public Intersection(Vector2 position, List<Road> roads)
        {
            this.position = position;
            this.roads = roads;
        }

        public Intersection(Vector2 position, Road[] roads)
        {
            this.position = position;
            this.roads = new List<Road>(roads);
        }

        public Intersection(Vector2 position)
        { 
            this.position = position;
            this.roads = new List<Road>();
        }

        public override string ToString()
        {
            string str = this.position.ToString() + " : [";
            for (int i = 0; i < this.roads.Count - 1; i++)
            {
                str += this.roads[i].ToString() + ",  ";
            }
            str += this.roads[this.roads.Count - 1].ToString() + "]";
            return str;
        }

        public override bool Equals(object? obj)
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

        public Road this[int id]
        {
            get { return this.roads[id]; }
            set { this.roads[id] = value; }
        }
    }


    public class DuplicateIntersectionException : Exception
    {
        public DuplicateIntersectionException() : base("Duplicate intersections are not allowed!") { }
        public DuplicateIntersectionException(string message) : base(message) { }
    }


    /// <summary>
    /// Field that holds all intersections, aka. graph
    /// </summary>
    private List<Intersection> map;


    public Map(List<Intersection> map)
    {
        this.map = map;
    }

    public Map(Intersection[] map) 
    {
        this.map = new List<Intersection>(map);
    }

    public Map(Intersection intersection) 
    {
        this.map = new List<Intersection> { intersection };
    }

    public Map(string path)
    {
        throw new NotImplementedException();
    }

    public Map() 
    {
        this.map = new List<Intersection>();
    }

    public void ToFile(string path)
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        string str = string.Empty;
        foreach (Intersection intersection in this.map) 
        {
            str += intersection.ToString() + "\n";
        }
        return str;
    }

    public bool ContainsIntersection(Intersection intersection)
    {
        return this.map.Contains(intersection);         // This should work thanks to custom hash functions
        // return this.map.Any(inter => inter.position== intersection.position);
    }

    public bool ContainsIntersection(Vector2 position)
    {
        return this.map.Any(inter => inter.position== position);
    }

    public indexT GetIntersectionIdx(Intersection intersection)
    {
        return this.map.IndexOf(intersection);          // This should work thanks to custom hash functions
        // return this.map.FindIndex(inter => inter.position == intersection.position);
    }

    public indexT GetIntersectionIdx(Vector2 position)
    {
        return this.map.FindIndex(inter => inter.position == position);
    }

    public void AddIntersection(Intersection intersection)
    {
        if (this.ContainsIntersection(intersection)) 
        {
            throw new DuplicateIntersectionException();
        }
        else
        {
            indexT thisIdx = this.map.Count;
            this.map.Add(intersection);                 // Add intersection
            foreach (Road road in intersection.roads)   // Add remaining connections
            {
                this.map[road.targetIdx].roads.Add(new Road(thisIdx, road.distance));
            }
        }
    }

    public void AddIntersection(Vector2 position, List<Road> roads)
    {
        this.AddIntersection(new Intersection(position, roads));
    }

    public void AddIntersection(Vector2 position, Road[] roads)
    {
        this.AddIntersection(new Intersection(position, roads));
    }

    public void UpdateIntersection(Intersection intersection)
    {
        indexT thisIdx = this.GetIntersectionIdx(intersection);
        if (thisIdx < 0)
        {
            throw new IndexOutOfRangeException();
        }
        else
        {
            foreach (Road road in this.map[thisIdx].roads)  // Less secure but faster for big maps
            {
                this.map[road.targetIdx].roads.RemoveAll(conn => conn.targetIdx == thisIdx);
            }
            //foreach (Intersection inter in this.map)
            //{
            //    inter.roads.RemoveAll(conn => conn.targetIdx == thisIdx);
            //}
            this.map[thisIdx] = intersection;
            foreach (Road road in intersection.roads)
            {
                this.map[road.targetIdx].roads.Add(new Road(thisIdx, road.distance));
            }
        }
    }

    public void UpdateIntersection(Vector2 position, List<Road> roads)
    {
        this.UpdateIntersection(new Intersection(position, roads));
    }

    public void UpdateIntersection(Vector2 position, Road[] roads)
    {
        this.UpdateIntersection(new Intersection(position, roads));
    }

    public void RemoveIntersection(indexT idx)
    {
        if (0 <= idx && idx < this.map.Count)
        {
            this.map.RemoveAt(idx);
            // Here we can't escape looping over entire graph/map
            foreach(Intersection intersection in this.map)
            {
                for (int i=0; i<intersection.roads.Count; i++)
                {
                    if (intersection.roads[i].targetIdx == idx) { intersection.roads.RemoveAt(i); }
                    else if (intersection.roads[i].targetIdx > idx) { intersection.roads[i] = new Road(intersection.roads[i].targetIdx-1, intersection.roads[i].distance); }
                }
            }
        }
        else
        {
            throw new IndexOutOfRangeException();
        }
    }

    public void RemoveIntersection(Vector2 position)
    {
        this.RemoveIntersection(this.GetIntersectionIdx(position));
    }

    public void RemoveIntersection(Intersection intersection)
    {
        this.RemoveIntersection(this.GetIntersectionIdx(intersection));
    }

    public void AddRoad(indexT idx1, indexT idx2, distanceT distance)
    {
        if ((0 <= idx1 && idx1 < this.map.Count) && (0 <= idx2 && idx2 < this.map.Count)) 
        {
            this.map[idx1].roads.Add(new Road(idx2, distance));
            this.map[idx2].roads.Add(new Road(idx1, distance));
        }
        else 
        { 
            throw new IndexOutOfRangeException(); 
        }
    }

    public void UpdateRoad(indexT idx1, indexT idx2, distanceT distance)
    {
        if ((0 <= idx1 && idx1 < this.map.Count) && (0 <= idx2 && idx2 < this.map.Count))
        {
            // At this point I've realized I could've used named tuples
            // I could also make Road a class, but I prefer safety of copying rather than having some problems with references
            for (int i=0; i < this.map[idx1].roads.Count; i++)
            {
                if (this.map[idx1].roads[i].targetIdx == idx2) { this.map[idx1].roads[i] = new Road(this.map[idx1].roads[i].targetIdx, distance); }
            }
            for (int i = 0; i < this.map[idx2].roads.Count; i++)
            {
                if (this.map[idx2].roads[i].targetIdx == idx1) { this.map[idx2].roads[i] = new Road(this.map[idx2].roads[i].targetIdx, distance); }
            }
        }
        else
        {
            throw new IndexOutOfRangeException();
        }
    }

    public void RemoveRoad(indexT idx1, indexT idx2)
    {
        if ((0 <= idx1 && idx1 < this.map.Count) && (0 <= idx2 && idx2 < this.map.Count))
        {
            this.map[idx1].roads.RemoveAll(r => r.targetIdx == idx2);
            this.map[idx2].roads.RemoveAll(r => r.targetIdx == idx1);
        }
        else
        {
            throw new IndexOutOfRangeException();
        }
    }

    public Intersection this[indexT i]
    {
        get { return this.map[i]; }
        set { this.UpdateIntersection(value); }
    }

    public distanceT this[indexT i, indexT j]
    {
        get { return this.map[i].roads[j].distance; }
        set { this.UpdateRoad(i, j, value); }
    }
}
