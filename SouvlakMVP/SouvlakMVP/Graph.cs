namespace SouvlakMVP;

using System.Numerics;
using System.Linq;
using indexT = System.Int32;
using edgeWeightT = System.Single;    // no maidens?
using edgeCountT = System.Int32;
using System.Text.Json.Serialization;
using System.Text.Json;


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
    /// Class responsible for holding data about single connection between two vertices
    /// </summary>
    public struct Edge
    {
        public indexT targetIdx { get; set; }
        public edgeWeightT weight { get; set; }
        public edgeCountT count { get; set; }

        /// <summary>
        /// Initializes a new instance of Edge class
        /// </summary>
        /// <param name="targetIdx">Id of vertex that edge connects to</param>
        /// <param name="weight">Weight of the edge</param>
        /// <param name="count">How many edges to create. Multiple edges between two wertices are allowed, as long as they have the same weight</param>
        public Edge(indexT targetIdx, edgeWeightT weight, edgeCountT count = 1)
        {
            this.targetIdx = targetIdx;
            this.weight = weight;
            this.count = count;
        }

        /// <summary>
        /// Represents edge in a form of a string
        /// </summary>
        /// <returns>String representation of an edge</returns>
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

        /// <summary>
        /// Performs a deep copy of the current object
        /// </summary>
        /// <returns>A new instance of the object with all of its properties deeply copied</returns>
        public Edge DeepCopy()
        {
            return new Edge(this.targetIdx, this.weight, this.count);;
        }
    }


    /// <summary>
    /// Struct for holding data of a single intersection, that is it's position and list of roads (connections)
    /// </summary>
    public struct Vertex
    {
        public Vector2 position { get; set; }
        public List<Edge> edgeList { get; set; }

        /// <summary>
        /// Initializes new instance of Vertex class
        /// </summary>
        /// <param name="position">Position on 2D map (must be unique)</param>
        /// <param name="edgeList">List of connection between this vertex and the other ones</param>
        public Vertex(Vector2 position, List<Edge> edgeList)
        {
            this.position = position;
            this.edgeList = edgeList;
        }

        /// <summary>
        /// Initializes new instance of Vertex class
        /// </summary>
        /// <param name="position">Position on 2D map (must be unique)</param>
        /// <param name="edgeList">Array of connection between this vertex and the other ones</param>
        public Vertex(Vector2 position, Edge[] edgeList)
        {
            this.position = position;
            this.edgeList = new List<Edge>(edgeList);
        }

        /// <summary>
        /// Initializes new instance of Vertex class
        /// </summary>
        /// <param name="position">Position on 2D map (must be unique)</param>
        public Vertex(Vector2 position)
        {
            this.position = position;
            this.edgeList = new List<Edge>();
        }

        /// <summary>
        /// Represents vertex in a string form
        /// </summary>
        /// <returns>String representation of an vertex</returns>
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
                str = str.Remove(str.Length - 3) + "]";
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

        /// <summary>
        /// Check whether vertex contains an edge with given target idx
        /// </summary>
        /// <param name="targetIdx">Idx of vertex, to which we want to check connection</param>
        /// <returns>True if connection exists, otherwise false</returns>
        public bool ContainsEdge(indexT targetIdx)
        {
            return this.edgeList.Any(e => e.targetIdx == targetIdx);
        }

        /// <summary>
        /// Check whether vertex contains a specific edge
        /// </summary>
        /// <param name="targetIdx">Edge that existence we want to confirm</param>
        /// <returns>True if connection exists, otherwise false</returns>
        public bool ContainsEdge(Edge edge)
        {
            return this.edgeList.Contains(edge);
        }

        /// <summary>
        /// Get edge from [this] Vertex to given target Vertex
        /// </summary>
        /// <param name="targetIdx">Target Idx of an edge we want to get</param>
        /// <returns>Edge ogject if found</returns>
        /// <exception cref="ArgumentNullException">Thrown if edge does not exist</exception>
        public Edge GetEdge(indexT targetIdx)
        {
            return this.edgeList.Find(e => e.targetIdx == targetIdx);
        }

        /// <summary>
        /// Add an edge to a vertex. Can be called on already existing edge if they both have the same weight (in which case edge counts will be added)
        /// </summary>
        /// <param name="edge">Edge that will be added</param>
        /// <exception cref="DuplicateEdgeException">Thrown if (in this vertex) edge with the same targetIdx, but different weight already exits</exception>
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

        /// <summary>
        /// Add an edge to a vertex. Can be called on already existing edge if they both have the same weight (in which case edge counts will be added)
        /// </summary>
        /// <param name="targetIdx">Target idx of a new edge</param>
        /// <param name="weight">Weight of a new edge</param>
        /// <param name="count">How many times this edge shall be added</param>
        /// <exception cref="DuplicateEdgeException">Thrown if (in this vertex) edge with the same targetIdx, but different weight already exits</exception>
        public void AddEdge(indexT targetIdx, edgeWeightT weight, edgeCountT count = 1)
        {
            this.AddEdge(new Edge(targetIdx, weight, count));
        }

        /// <summary>
        /// Add given edge count to an edge count of an already existing edge
        /// </summary>
        /// <param name="targetIdx">Target Idx of an edge we want to update</param>
        /// <param name="count">New edge count</param>
        /// <exception cref="IndexOutOfRangeException">Thrown if edge we want to update does not exist</exception>
        public void IncrementEdgeCount(indexT targetIdx, edgeCountT count = 1)
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
                                                  this.edgeList[edgeIdx].count + count);   // once again, in C# structs are copied by default
            }
        }
        
        /// <summary>
        /// Update an already esisting edge with new weight and count
        /// </summary>
        /// <param name="edge">Data of a new edge</param>
        /// <exception cref="IndexOutOfRangeException">Thrown if edge we want to update does not exist</exception>
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
        /// <summary>
        /// Update an already esisting edge with new weight and count
        /// </summary>
        /// <param name="targetIdx">Target Idx of an edge we want to update</param>
        /// <param name="weight">New edge weighr</param>
        /// <param name="count">New edge count</param>
        /// <exception cref="IndexOutOfRangeException">Thrown if edge we want to update does not exist</exception>
        public void UpdateEdge(indexT targetIdx, edgeWeightT weight, edgeCountT count = 1)
        {
            this.UpdateEdge(new Edge(targetIdx, weight, count));
        }

        /// <summary>
        /// Update edge count of an already existing edge
        /// </summary>
        /// <param name="targetIdx">Target Idx of an edge we want to update</param>
        /// <param name="count">New edge count</param>
        /// <exception cref="IndexOutOfRangeException">Thrown if edge we want to update does not exist</exception>
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

        /// <summary>
        /// Decrements target Idxs of all edges, that are greater than given treshold. Used after another vertex was removed
        /// </summary>
        /// <param name="threshold">Value above which target Idxs will be decremented</param>
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

        /// <summary>
        /// Remove edge (or simply lower it's count)
        /// </summary>
        /// <param name="targetIdx">Target Idxs of an edge we want to modify or remove</param>
        /// <param name="count">By how mych edge's count should be lowered. If results is lower than one, edge will be removed</param>
        /// <exception cref="IndexOutOfRangeException">Thrown if edge does not exist</exception>
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

        /// <summary>
        /// Remove edge (or simply lower it's count)
        /// </summary>
        /// <param name="edge">Edge that should be removed, or which count should be lowered</param>
        /// <exception cref="IndexOutOfRangeException">Thrown if edge does not exist</exception>
        public void RemoveEdge(Edge edge)
        {
            this.RemoveEdge(edge.targetIdx, edge.count);
        }

        /// <summary>
        /// Remove edge
        /// </summary>
        /// <param name="targetIdx">Target Idxs of an edge we want to remove</param>
        /// <param name="RemoveAll">Cofirm deletion (needed only so that overload will not get funky)</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
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

        /// <summary>
        /// Get number of edges (with their copies) connected with this, particular vertex
        /// </summary>
        /// <returns>Number of edges and their copies</returns>
        public edgeCountT GetEdgeCount()
        {
            return this.edgeList.Sum(e => e.count);
        }

        /// <summary>
        /// Get an edge with given target Idx
        /// </summary>
        /// <param name="idx">Target Idx of an edge</param>
        /// <returns>Edge object connecting this vertex with the one with index equal to "idx" parameter</returns>
        public Edge this[indexT idx]
        {
            get { return this.GetEdge(idx); }
            set { this.UpdateEdge(value); }
        }

        /// <summary>
        /// Performs a deep copy of the current object
        /// </summary>
        /// <returns>A new instance of the object with all of its properties deeply copied</returns>
        public Vertex DeepCopy() 
        {
            Vector2 deepCopyVector2 = new Vector2(this.position.X, this.position.Y);;
            List<Edge> deepCopyEdges = new List<Edge>();

            for (int i = 0; i < this.edgeList.Count; i++)
            {
                deepCopyEdges.Add(this.edgeList[i].DeepCopy());
            }

            return new Vertex(deepCopyVector2, deepCopyEdges);;
        }
    }


    /// <summary>
    /// Field that holds all intersections, aka. graph
    /// </summary>
    private List<Vertex> graph;

    /// <summary>
    /// Create new instance of Graph class
    /// </summary>
    /// <param name="graph">List of vertices</param>
    public Graph(List<Vertex> graph)
    {
        this.graph = graph;
    }

    /// <summary>
    /// Create new instance of Graph class
    /// </summary>
    /// <param name="graph">Array of vertices</param>
    public Graph(Vertex[] graph)
    {
        this.graph = new List<Vertex>(graph);
    }

    /// <summary>
    /// Create new instance of Graph class
    /// </summary>
    /// <param name="vertex">Single, starting vertex</param>
    public Graph(Vertex vertex)
    {
        this.graph = new List<Vertex> { vertex };
    }

    /// <summary>
    /// Create new instance of Graph class
    /// </summary>
    /// <param name="path">Path to JSON file containing graph's structure</param>
    public Graph(string path)
    {
        JsonSerializerOptions _options = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new Vector2Converter() }
        };
        using FileStream graphJSON = File.OpenRead(path);
        List<Vertex>? tempGraph = JsonSerializer.Deserialize<List<Vertex>>(graphJSON, _options);
        if (tempGraph == null)
        {
            throw new Exception("Json file is not correct");
        }
        this.graph = tempGraph;
    }

    /// <summary>
    /// Create new instance of Graph class
    /// </summary>
    public Graph()
    {
        this.graph = new List<Vertex>();
    }

    /// <summary>
    /// Write this graph's structure into JSON file
    /// </summary>
    /// <param name="path"></param>
    public void ToFile(string path)
    {
        JsonSerializerOptions _options = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new Vector2Converter() }
            // WriteIndented = true
        };

        var jsonString = JsonSerializer.Serialize(this.graph, _options);
        File.WriteAllText(path, jsonString);
    }

    /// <summary>
    /// Get string representation of a graph
    /// </summary>
    /// <returns>String representation of a graph</returns>
    public override string ToString()
    {
        string str = string.Empty;
        foreach (Vertex v in this.graph)
        {
            str += v.ToString() + "\n";
        }
        return str;
    }

    /// <summary>
    /// Get number of vertices in  this graph
    /// </summary>
    /// <returns>nNumber of vertices in  this graph</returns>
    public int GetVertexCount()
    {
        return this.graph.Count;
    }

    /// <summary>
    /// Get total count of edges in this graph, inculding copied edges
    /// </summary>
    /// <returns>Get total count of edges in this graph, inculding copied edges</returns>
    public edgeCountT GetEdgeCount()
    {
        edgeCountT totalCount = 0;
        foreach (Vertex v in this.graph)
        {
            totalCount += v.GetEdgeCount();
        }
        return totalCount;
    }

    /// <summary>
    /// Get a list o vertices with uneven edge count
    /// </summary>
    /// <returns>List o vertices with uneven edge count</returns>
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

    /// <summary>
    /// Get a list o vertices' indices that have uneven edge count
    /// </summary>
    /// <returns>List o vertices' indices that have uneven edge count</returns>
    public List<indexT> GetUnevenVerticesIdxs()
    {
        List<indexT> verticesIdxs = new List<indexT>();
        for (indexT i=0; i<this.graph.Count(); i++)
        {
            if (this.graph[i].GetEdgeCount() % 2 != 0)
            {
                verticesIdxs.Add(i);
            }
        }
        return verticesIdxs;
    }

    /// <summary>
    /// Check whether vertex exists in graph
    /// </summary>
    /// <param name="position">Position of a vertex, we want to check if exists</param>
    /// <returns>True if vertex was found, otherwise false</returns>
    public bool ContainsVertex(Vector2 position)
    {
        return this.graph.Any(e => e.position == position);
    }

    /// <summary>
    /// Check whether vertex exists in graph
    /// </summary>
    /// <param name="Vertex">Vertex that we want to check if exists</param>
    /// <returns>True if vertex was found, otherwise false</returns>
    public bool ContainsVertex(Vertex Vertex)
    {
        return this.graph.Contains(Vertex);
    }

    /// <summary>
    /// Get index of a certain vertex
    /// </summary>
    /// <param name="position">Position of a vertex, which index we want to get</param>
    /// <returns>Index of a vertex</returns>
    public indexT GetVertexIdx(Vector2 position)
    {
        return this.graph.FindIndex(e => e.position == position);
    }

    /// <summary>
    /// Get index of a certain vertex
    /// </summary>
    /// <param name="Vertex">Vertex which index we want to get</param>
    /// <returns>Index of a vertex</returns>
    public indexT GetVertexIdx(Vertex Vertex)
    {
        return this.graph.IndexOf(Vertex);
    }

    /// <summary>
    /// Get vertex whith given index
    /// </summary>
    /// <param name="idx">Index of wanted vertex</param>
    /// <returns>Vertex we want to get</returns>
    public Vertex GetVertex(indexT idx)
    {
        return this.graph[idx];
    }

    /// <summary>
    /// Add vertex to a graph and automatically update all necessary edges
    /// </summary>
    /// <param name="vertex">Vertex we want to add</param>
    /// <exception cref="DuplicateVertexException">Thrown if vertex with the same position already exists</exception>
    /// <exception cref="IndexOutOfRangeException">Thrown if added vertex contains connection to a vertex that does not exist yet</exception>
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

    /// <summary>
    /// Add vertex to a graph and automatically update all necessary edges
    /// </summary>
    /// <param name="position">Positon of an vertex that will be added</param>
    /// <param name="edgeList">List of edges of a new vertex</param>
    /// <exception cref="DuplicateVertexException">Thrown if vertex with the same position already exists</exception>
    /// <exception cref="IndexOutOfRangeException">Thrown if added vertex contains connection to a vertex that does not exist yet</exception>
    public void AddVertex(Vector2 position, List<Edge> edgeList)
    {
        this.AddVertex(new Vertex(position, edgeList));
    }

    /// <summary>
    /// Add vertex to a graph and automatically update all necessary edges
    /// </summary>
    /// <param name="position">Positon of an vertex that will be added</param>
    /// <param name="edgeList">Array of edges of a new vertex</param>
    /// <exception cref="DuplicateVertexException">Thrown if vertex with the same position already exists</exception>
    /// <exception cref="IndexOutOfRangeException">Thrown if added vertex contains connection to a vertex that does not exist yet</exception>
    public void AddVertex(Vector2 position, Edge[] edgeList)
    {
        this.AddVertex(new Vertex(position, edgeList));
    }

    /// <summary>
    /// Update edges of a given vertex, and all vertices connected to this vertex
    /// </summary>
    /// <param name="vertex">Vertex which edges will be updated</param>
    /// <exception cref="IndexOutOfRangeException">Thrown if vertex does not exist, or contains an edge, that connects with an non-exsitent vertex</exception>
    public void UpdateVertex(Vertex vertex)
    {
        indexT thisIdx = this.GetVertexIdx(vertex);
        if (thisIdx < 0)
        {
            throw new IndexOutOfRangeException("Vertex with this position does not exist!");
        }
        else
        {
            if (vertex.edgeList.Any(e => e.targetIdx >= this.GetVertexCount()))
            {
                throw new IndexOutOfRangeException("Vertex contains a edge, that connects with an non-existent vertex!");
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

    /// <summary>
    /// Update edges of a given vertex, and all vertices connected to this vertex
    /// </summary>
    /// <param name="position">Position of an edge to update</param>
    /// <param name="edgeList">List of edges in an updated vertex</param>
    /// <exception cref="IndexOutOfRangeException">Thrown if vertex does not exist, or contains an edge, that connects with an non-exsitent vertex</exception>
    public void UpdateVertex(Vector2 position, List<Edge> edgeList)
    {
        this.UpdateVertex(new Vertex(position, edgeList));
    }

    /// <summary>
    /// Update edges of a given vertex, and all vertices connected to this vertex
    /// </summary>
    /// <param name="position">Position of an edge to update</param>
    /// <param name="edgeList">Array of edges in an updated vertex</param>
    /// <exception cref="IndexOutOfRangeException">Thrown if vertex does not exist, or contains an edge, that connects with an non-exsitent vertex</exception>
    public void UpdateVertex(Vector2 position, Edge[] edgeList)
    {
        this.UpdateVertex(new Vertex(position, edgeList));
    }

    /// <summary>
    /// Remove a vertex and all of it's connetions (in both ways)
    /// </summary>
    /// <param name="idx">Index of a vertex that will be removed</param>
    /// <exception cref="IndexOutOfRangeException">Thrown if vertex does not exist</exception>
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

    /// <summary>
    /// Remove a vertex and all of it's connetions (in both ways)
    /// </summary>
    /// <param name="position">Position of a vertex that will be removed</param>
    /// <exception cref="IndexOutOfRangeException">Thrown if vertex does not exist</exception>
    public void RemoveVertex(Vector2 position)
    {
        this.RemoveVertex(this.GetVertexIdx(position));
    }

    /// <summary>
    /// Remove a vertex and all of it's connetions (in both ways)
    /// </summary>
    /// <param name="vertex">Vertex that will be removed</param>
    /// <exception cref="IndexOutOfRangeException">Thrown if vertex does not exist</exception>
    public void RemoveVertex(Vertex vertex)
    {
        this.RemoveVertex(this.GetVertexIdx(vertex));
    }

    /// <summary>
    /// Check whether graph contains an edge that starts at "idx1" and ends at "idx2"
    /// </summary>
    /// <param name="idx1">First index</param>
    /// <param name="idx2">Second index</param>
    /// <returns>True if edge exists, otherwise false</returns>
    public bool ContainsEdge(indexT idx1, indexT idx2)
    {
        if (0 <= idx1 && idx1 < this.GetVertexCount())
        {
            return this.graph[idx1].ContainsEdge(idx2);
        }
        return false;
    }

    /// <summary>
    /// Get an edge, that connects vertices with indices "idx1" and "idx2:
    /// </summary>
    /// <param name="idx1">First index</param>
    /// <param name="idx2">Second index/param>
    /// <returns>Edge connecting two vertices</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown if edge does not exist</exception>
    public Edge GetEdge(indexT idx1, indexT idx2)
    {
        if (0 <= idx1 && idx1 < this.GetVertexCount())
        {
            return this.graph[idx1].edgeList[idx2];
        }
        throw new IndexOutOfRangeException("Vertex does not exist!");
    }

    /// <summary>
    /// Add two-way edge between two vertices
    /// </summary>
    /// <param name="idx1">Index of a first vertex</param>
    /// <param name="idx2">Index of a second vertex</param>
    /// <param name="weight">Weight of an edge</param>
    /// <param name="count">Edge's count (by default equal to 1)</param>
    /// <exception cref="IndexOutOfRangeException">Thrown if at one of given vertices or given edge does not exist</exception>
    /// <exception cref="DuplicateEdgeException">Thrown if an edge connecting those two vertices already exists and has different weight</exception>
    public void AddEdge(indexT idx1, indexT idx2, edgeWeightT weight, edgeCountT count = 1)
    {
        if ((0 <= idx1 && idx1 < this.GetVertexCount()) && (0 <= idx2 && idx2 < this.GetVertexCount()))
        {
            this.graph[idx1].AddEdge(idx2, weight, count);
            this.graph[idx2].AddEdge(idx1, weight, count);
        }
        else
        {
            throw new IndexOutOfRangeException("Vertex does not exist!");
        }
    }

    /// <summary>
    /// Increment count of given two-way edge
    /// </summary>
    /// <param name="idx1">Index of a first vertex</param>
    /// <param name="idx2">Index of a second vertex</param>
    /// <param name="count">Edge's count (by default equal to 1)</param>
    /// <exception cref="IndexOutOfRangeException">Thrown if at least one of given vertices does not exist</exception>
    public void IncrementEdgeCount(indexT idx1, indexT idx2, edgeCountT count = 1)
    {
        if ((0 <= idx1 && idx1 < this.GetVertexCount()) && (0 <= idx2 && idx2 < this.GetVertexCount()))
        {
            this.graph[idx1].IncrementEdgeCount(idx2, count);
            this.graph[idx2].IncrementEdgeCount(idx1, count);
        }
        else
        {
            throw new IndexOutOfRangeException("Vertex does not exist!");
        }
    }

    /// <summary>
    /// Update two-way edge between two vertices
    /// </summary>
    /// <param name="idx1">Index of a first vertex</param>
    /// <param name="idx2">Index of a second vertex</param>
    /// <param name="weight">Weight of an edge</param>
    /// <param name="count">Edge's count (by default equal to 1)</param>
    /// <exception cref="IndexOutOfRangeException">Thrown if either vertex or an edge does not exist</exception>
    public void UpdateEdge(indexT idx1, indexT idx2, edgeWeightT weight, edgeCountT count = 1)
    {
        if ((0 <= idx1 && idx1 < this.GetVertexCount()) && (0 <= idx2 && idx2 < this.GetVertexCount()))
        {
            this.graph[idx1].UpdateEdge(idx2, weight, count);
            this.graph[idx2].UpdateEdge(idx1, weight, count);
        }
        else
        {
            throw new IndexOutOfRangeException("Vertex does not exist!");
        }
    }

    /// <summary>
    /// Update count number of a two-way edge between two vertices
    /// </summary>
    /// <param name="idx1">Index of a first vertex</param>
    /// <param name="idx2">Index of a second vertex</param>
    /// <param name="count">Edge's count (by default equal to 1)</param>
    /// <exception cref="IndexOutOfRangeException">Thrown if either vertex or an edge does not exist</exception>
    public void UpdateEdgeCount(indexT idx1, indexT idx2, edgeCountT count)
    {
        if ((0 <= idx1 && idx1 < this.GetVertexCount()) && (0 <= idx2 && idx2 < this.GetVertexCount()))
        {
            this.graph[idx1].UpdateEdgeCount(idx2, count);
            this.graph[idx2].UpdateEdgeCount(idx1, count);
        }
        else
        {
            throw new IndexOutOfRangeException("Vertex does not exist!");
        }
    }

    /// <summary>
    /// Remove two-way edge between two vertices
    /// </summary>
    /// <param name="idx1">Index of a first vertex</param>
    /// <param name="idx2">Index of a second vertex</param>
    /// <exception cref="IndexOutOfRangeException">Thrown if either vertex or edge does not exist</exception>
    public void RemoveEdge(indexT idx1, indexT idx2)
    {
        if ((0 <= idx1 && idx1 < this.GetVertexCount()) && (0 <= idx2 && idx2 < this.GetVertexCount()))
        {
            this.graph[idx1].RemoveEdge(idx2);
            this.graph[idx2].RemoveEdge(idx1);
        }
        else
        {
            throw new IndexOutOfRangeException("Vertex does not exist!");
        }
    }

    /// <summary>
    /// Get or update vertex whith given index
    /// </summary>
    /// <param name="idx">Index of wanted vertex</param>
    /// <returns>Vertex we want to get</returns>
    public Vertex this[indexT idx]
    {
        get { return this.GetVertex(idx); }
        set { this.UpdateVertex(value); }
    }

    /// <summary>
    /// Get or update two-way edge connecting two vertices
    /// </summary>
    /// <param name="idx1">Index of a first vertex</param>
    /// <param name="idx2">Index of a second vertex</param>
    /// <returns>Edge connecting two vertices</returns>
    public Edge this[indexT idx1, indexT idx2]
    {
        get { return this.GetEdge(idx1, idx2); }
        set { this.UpdateEdge(idx1, idx2, value.weight, value.count); }
    }

    /// <summary>
    /// Performs a deep copy of the current object
    /// </summary>
    /// <returns>A new instance of the object with all of its properties deeply copied</returns>
    public Graph DeepCopy() 
    {
        List<Vertex> deepCopyVertices = new List<Vertex>();

        for(int i = 0; i < this.graph.Count; i++)
        {
            deepCopyVertices.Add(this.graph[i].DeepCopy());
        }

        return new Graph(deepCopyVertices);;
    }
}
