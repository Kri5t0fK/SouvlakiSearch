namespace SouvlakMVP;

using System.Numerics;
using System.Linq;
using indexT = System.Int32;
using edgeWeightT = System.Single;    // no maidens?
using edgeCountT = System.Int32;


/// <summary>
/// Class for storing city's map (aka. graph in which vertices = intersections and edges = roads)
/// </summary>
public class Graph
{
    public class DuplicateEdgeException : Exception
    {
        public DuplicateEdgeException() : base("Duplicate edges with different weights are not allowed!") { }
        public DuplicateEdgeException(string message) : base(message) { }
    }


    public class DuplicateVertexException : Exception
    {
        public DuplicateVertexException() : base("Duplicate intersections are not allowed!") { }
        public DuplicateVertexException(string message) : base(message) { }
    }


    /// <summary>
    /// Class responsible for holding data about single connection, aka. target's ID and distance 
    /// </summary>
    public struct Edge
    {
        public indexT targetIdx;
        public edgeWeightT weight;
        public edgeCountT count;

        public Edge(indexT targetIdx, edgeWeightT weight, edgeCountT count = 1)
        {
            this.targetIdx = targetIdx;
            this.weight = weight;
            this.count = count;
        }

        public override string ToString()
        {
            return this.targetIdx.ToString() + " : " + this.count.ToString() + " x " + this.weight.ToString("n2");
        }

        public override bool Equals(object? obj)
        {
            if (obj is not null and Edge)
            {
                Edge other = (Edge)obj;
                return this.targetIdx == other.targetIdx;
            }
            else
            {
                return false;
            };
        }

        public override int GetHashCode()
        {
            return this.targetIdx.GetHashCode() * 997 + this.weight.GetHashCode();    // just multiply one value by big prime number
        }

        public static bool operator ==(Edge r1, Edge r2) => r1.targetIdx == r2.targetIdx;
        public static bool operator !=(Edge r1, Edge r2) => r1.targetIdx != r2.targetIdx;
    }


    /// <summary>
    /// Struct for holding data of a single intersection, that is it's position and list of roads (connections)
    /// </summary>
    public struct Vertex
    {
        public Vector2 position;
        public List<Edge> edgeList;

        public Vertex(Vector2 position, List<Edge> edgeList)
        {
            this.position = position;
            this.edgeList = edgeList;
        }

        public Vertex(Vector2 position, Edge[] edgeList)
        {
            this.position = position;
            this.edgeList = new List<Edge>(edgeList);
        }

        public Vertex(Vector2 position)
        {
            this.position = position;
            this.edgeList = new List<Edge>();
        }

        public override string ToString()
        {
            if (this.edgeList.Count == 0)
            {
                return "[]";
            }
            else
            {
                string str = this.position.ToString() + " : [";
                foreach (Edge e in this.edgeList)
                {
                    str += e.ToString() + ";  ";
                }
                str = str.Remove(str.Length - 3);
                return str;
            }
        }

        public override bool Equals(object? obj)
        {
            if (obj is not null and Vertex)
            {
                Vertex other = (Vertex)obj;
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

        public static bool operator ==(Vertex i1, Vertex i2) => i1.position == i2.position;
        public static bool operator !=(Vertex i1, Vertex i2) => i1.position != i2.position;

        public bool ContainsEdge(indexT targetIdx)
        {
            return this.edgeList.Any(e => e.targetIdx == targetIdx);
        }

        public bool ContainsEdge(Edge edge)
        {
            return this.edgeList.Contains(edge);
        }

        public Edge GetEdge(indexT targetIdx)
        {
            return this.edgeList.Find(e => e.targetIdx == targetIdx);
        }

        public void AddEdge(Edge edge)
        {
            indexT edgeIdx = this.edgeList.IndexOf(edge);
            if (edgeIdx == -1)
            {
                this.edgeList.Add(edge);    // in C# structs are copied by default
            }
            else if (this.edgeList[edgeIdx].weight == edge.weight)
            {
                this.edgeList[edgeIdx] = new Edge(this.edgeList[edgeIdx].targetIdx,
                                                  this.edgeList[edgeIdx].weight,
                                                  this.edgeList[edgeIdx].count + edge.count); // once again, in C# structs are copied by default
            }
            else
            {
                throw new DuplicateEdgeException();
            }
        }

        public void AddEdge(indexT targetIdx, edgeWeightT weight, edgeCountT count = 1)
        {
            this.AddEdge(new Edge(targetIdx, weight, count));
        }

        public void UpdateEdge(Edge edge)
        {
            indexT edgeIdx = this.edgeList.IndexOf(edge);
            if (edgeIdx == -1)
            {
                throw new IndexOutOfRangeException();
            }
            else
            {
                this.edgeList[edgeIdx] = edge;
            }
        }

        public void UpdateEdge(indexT targetIdx, edgeWeightT weight, edgeCountT count = 1)
        {
            this.UpdateEdge(new Edge(targetIdx, weight, count));
        }

        public void UpdateEdgeCount(indexT targetIdx, edgeCountT count)
        {
            indexT edgeIdx = this.edgeList.FindIndex(e => e.targetIdx == targetIdx);
            if (edgeIdx == -1)
            {
                throw new IndexOutOfRangeException();
            }
            else
            {
                this.edgeList[edgeIdx] = new Edge(this.edgeList[edgeIdx].targetIdx,
                                                  this.edgeList[edgeIdx].weight,
                                                  count);   // once again, in C# structs are copied by default
            }
        }


        public void DecrementTargetIdxsGreaterThan(indexT threshold)
        {
            for (int i = 0; i < this.edgeList.Count; i++)
            {
                if (this.edgeList[i].targetIdx > threshold)
                {
                    this.edgeList[i] = new Edge(this.edgeList[i].targetIdx - 1,
                                                this.edgeList[i].weight,
                                                this.edgeList[i].count);    // once again, in C# structs are copied by default
                }
            }
        }

        public void RemoveEdge(indexT targetIdx, edgeCountT count = 1)
        {
            indexT edgeIdx = this.edgeList.FindIndex(e => e.targetIdx == targetIdx);
            if (edgeIdx == -1)
            {
                throw new IndexOutOfRangeException();
            }
            else if (this.edgeList[edgeIdx].count - count > 0)
            {
                this.edgeList[edgeIdx] = new Edge(this.edgeList[edgeIdx].targetIdx,
                                                  this.edgeList[edgeIdx].weight,
                                                  this.edgeList[edgeIdx].count - count); // once again, in C# structs are copied by default
            }
            else
            {
                this.edgeList.RemoveAt(edgeIdx);
            }
        }

        public void RemoveEdge(Edge edge)
        {
            this.RemoveEdge(edge.targetIdx, edge.count);
        }

        public void RemoveEdge(indexT targetIdx, bool RemoveAll)
        {
            indexT edgeIdx = this.edgeList.FindIndex(e => e.targetIdx == targetIdx);
            if (edgeIdx == -1)
            {
                throw new IndexOutOfRangeException();
            }
            else if (RemoveAll)
            {
                this.edgeList.RemoveAt(edgeIdx);
            }
        }

        public edgeCountT GetEdgeCount()
        {
            return this.edgeList.Sum(e => e.count);
        }

        public Edge this[indexT idx]
        {
            get { return this.GetEdge(idx); }
            set { this.UpdateEdge(value); }
        }
    }


    /// <summary>
    /// Field that holds all intersections, aka. graph
    /// </summary>
    private List<Vertex> graph;

    public Graph(List<Vertex> graph)
    {
        this.graph = graph;
    }

    public Graph(Vertex[] graph)
    {
        this.graph = new List<Vertex>(graph);
    }

    public Graph(Vertex vertex)
    {
        this.graph = new List<Vertex> { vertex };
    }

    public Graph(string path)
    {
        throw new NotImplementedException();
    }

    public Graph()
    {
        this.graph = new List<Vertex>();
    }

    public void ToFile(string path)
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        string str = string.Empty;
        foreach (Vertex v in this.graph)
        {
            str += v.ToString() + "\n";
        }
        return str;
    }

    public int GetVertexCount()
    {
        return this.graph.Count;
    }

    public edgeCountT GetEdgeCount()
    {
        edgeCountT totalCount = 0;
        foreach (Vertex v in this.graph)
        {
            totalCount += v.GetEdgeCount();
        }
        return totalCount;
    }

    public List<Vertex> GetUnevenVertices()
    {
        List<Vertex> vertices = new List<Vertex>();
        foreach (Vertex v in this.graph)
        {
            if (v.GetEdgeCount() % 2 != 0)
            {
                vertices.Add(v);
            }
        }
        return vertices;
    }

    public bool ContainsVertex(Vector2 position)
    {
        return this.graph.Any(e => e.position == position);
    }

    public bool ContainsVertex(Vertex Vertex)
    {
        return this.graph.Contains(Vertex);
    }

    public indexT GetVertexIdx(Vector2 position)
    {
        return this.graph.FindIndex(e => e.position == position);
    }

    public indexT GetVertexIdx(Vertex Vertex)
    {
        return this.graph.IndexOf(Vertex);
    }

    public Vertex GetVertex(indexT idx)
    {
        return this.graph[idx];
    }

    public void AddVertex(Vertex vertex)
    {
        if (this.ContainsVertex(vertex))
        {
            throw new DuplicateVertexException();
        }
        else
        {
            indexT thisIdx = this.GetVertexCount();
            if (vertex.edgeList.Any(e => e.targetIdx >= thisIdx))
            {
                throw new IndexOutOfRangeException();
            }
            else
            {
                this.graph.Add(vertex);                 // Add vertex
                foreach (Edge e in vertex.edgeList)     // Add remaining edges
                {
                    this.graph[e.targetIdx].AddEdge(thisIdx, e.weight, e.count);
                }
            }
        }
    }

    public void AddVertex(Vector2 position, List<Edge> edgeList)
    {
        this.AddVertex(new Vertex(position, edgeList));
    }

    public void AddVertex(Vector2 position, Edge[] edgeList)
    {
        this.AddVertex(new Vertex(position, edgeList));
    }

    public void UpdateVertex(Vertex vertex)
    {
        indexT thisIdx = this.GetVertexIdx(vertex);
        if (thisIdx < 0)
        {
            throw new IndexOutOfRangeException();
        }
        else
        {
            if (vertex.edgeList.Any(e => e.targetIdx >= this.GetVertexCount()))
            {
                throw new IndexOutOfRangeException();
            }
            else
            {
                foreach (Edge e in this.graph[thisIdx].edgeList)
                {
                    this.graph[e.targetIdx].RemoveEdge(thisIdx);
                }
                this.graph[thisIdx] = vertex;
                foreach (Edge e in vertex.edgeList)
                {
                    this.graph[e.targetIdx].AddEdge(thisIdx, e.weight, e.count);
                }
            }
        }
    }

    public void UpdateVertex(Vector2 position, List<Edge> edgeList)
    {
        this.UpdateVertex(new Vertex(position, edgeList));
    }

    public void UpdateVertex(Vector2 position, Edge[] edgeList)
    {
        this.UpdateVertex(new Vertex(position, edgeList));
    }

    public void RemoveVertex(indexT idx)
    {
        if (0 <= idx && idx < this.graph.Count)
        {
            this.graph.RemoveAt(idx);
            // Here we can't escape looping over entire graph/map
            foreach (Vertex v in this.graph)
            {
                v.RemoveEdge(idx);
                v.DecrementTargetIdxsGreaterThan(idx);
            }
        }
        else
        {
            throw new IndexOutOfRangeException();
        }
    }

    public void RemoveVertex(Vector2 position)
    {
        this.RemoveVertex(this.GetVertexIdx(position));
    }

    public void RemoveVertex(Vertex vertex)
    {
        this.RemoveVertex(this.GetVertexIdx(vertex));
    }

    public bool ContainsEdge(indexT idx1, indexT idx2)
    {
        if (0 <= idx1 && idx1 < this.GetVertexCount())
        {
            return this.graph[idx1].ContainsEdge(idx2);
        }
        return false;
    }

    public Edge GetEdge(indexT idx1, indexT idx2)
    {
        if (0 <= idx1 && idx1 < this.GetVertexCount())
        {
            return this.graph[idx1].edgeList[idx2];
        }
        throw new IndexOutOfRangeException();
    }

    public void AddEdge(indexT idx1, indexT idx2, edgeWeightT weight, edgeCountT count = 1)
    {
        if ((0 <= idx1 && idx1 < this.GetVertexCount()) && (0 <= idx2 && idx2 < this.GetVertexCount()))
        {
            this.graph[idx1].AddEdge(idx2, weight, count);
            this.graph[idx2].AddEdge(idx1, weight, count);
        }
        else
        {
            throw new IndexOutOfRangeException();
        }
    }

    public void UpdateEdge(indexT idx1, indexT idx2, edgeWeightT weight, edgeCountT count = 1)
    {
        if ((0 <= idx1 && idx1 < this.GetVertexCount()) && (0 <= idx2 && idx2 < this.GetVertexCount()))
        {
            this.graph[idx1].UpdateEdge(idx2, weight, count);
            this.graph[idx2].UpdateEdge(idx1, weight, count);
        }
        else
        {
            throw new IndexOutOfRangeException();
        }
    }

    public void UpdateEdgeCount(indexT idx1, indexT idx2, edgeCountT count)
    {
        if ((0 <= idx1 && idx1 < this.GetVertexCount()) && (0 <= idx2 && idx2 < this.GetVertexCount()))
        {
            this.graph[idx1].UpdateEdgeCount(idx2, count);
            this.graph[idx2].UpdateEdgeCount(idx1, count);
        }
        else
        {
            throw new IndexOutOfRangeException();
        }
    }

    public void RemoveEdge(indexT idx1, indexT idx2)
    {
        if ((0 <= idx1 && idx1 < this.GetVertexCount()) && (0 <= idx2 && idx2 < this.GetVertexCount()))
        {
            this.graph[idx1].RemoveEdge(idx2);
            this.graph[idx2].RemoveEdge(idx1);
        }
        else
        {
            throw new IndexOutOfRangeException();
        }
    }

    public Vertex this[indexT idx]
    {
        get { return this.GetVertex(idx); }
        set { this.UpdateVertex(value); }
    }

    public Edge this[indexT idx1, indexT idx2]
    {
        get { return this.GetEdge(idx1, idx2); }
        set { this.UpdateEdge(idx1, idx2, value.weight, value.count); }
    }
}
