namespace SouvlakMVP;

using System.Linq;
using indexT = System.Int32;
using edgeWeightT = System.Single;
using System.Linq.Expressions;

/// <summary>
/// Interface for easy use of dijkstra algorithm
/// </summary>
public class VerticesConnections
{
    /// <summary>
    /// Class representing a single connection, aka. weight and road
    /// </summary>
    public class Connection
    {
        private readonly edgeWeightT weight;
        public edgeWeightT Weight { get { return this.weight; } }
        private readonly List<indexT> path;
        public List<indexT> Path { get { return new List<indexT>(this.path); } }

        /// <summary>
        /// Initializes a new instance of Connection calss
        /// </summary>
        /// <param name="weight">Weight of connection</param>
        /// <param name="path">Connections' path in form of list of indices</param>
        public Connection(edgeWeightT weight, List<indexT> path)
        {
            this.weight = weight;
            this.path = path;
        }

        /// <summary>
        /// Return string containing only weight of a connection
        /// </summary>
        /// <returns>String representation of connection's weight</returns>
        public override string ToString()
        {
            return this.weight.ToString("n2");
        }

        /// <summary>
        /// Return string containing weight and path of a connection
        /// </summary>
        /// <returns>String representation of a full connection</returns>
        public string ToStringFull()
        {
            if (this.path.Count == 0)   // There shouldn't be a case where weight exists but path is empty, but hey
            {
                return this.weight.ToString("n2") + " : []";
            }
            else
            {
                string str = this.weight.ToString() + " : [";
                foreach (indexT idx in this.path)
                {
                    str += idx.ToString() + ", ";
                }
                str = str.Remove(str.Length - 2) + "]";
                return str;
            }
        }

        /// <summary>
        /// Faster (but still safe) way to access connection's path elements
        /// </summary>
        /// <param name="idx">Index of element in connection's path</param>
        /// <returns></returns>
        public indexT this[indexT idx]
        {
            get { return this.path[idx]; }
        }
    }


    private Graph graph;
    private Dictionary<indexT, indexT> indexTranslate;
    private Connection[,] connectionMatrix;


    /// <summary>
    /// Rebuild connection matrix. Call this method after you finish updating graph
    /// </summary>
    /// <param name="graph">Graph from which connection matrix will be created</param>
    public void Rebuild(Graph graph)
    {
        this.graph = graph;

        List<indexT> unevenVertices = this.graph.GetUnevenVerticesIdxs();
        this.connectionMatrix = new Connection[unevenVertices.Count, unevenVertices.Count];

        this.indexTranslate = new Dictionary<indexT, indexT>();
        for (indexT thisIdx = 0; thisIdx < unevenVertices.Count; thisIdx++)
        {
            this.indexTranslate.Add(unevenVertices[thisIdx], thisIdx);
        }
    }

    /// <summary>
    /// Initializes a new instance of VerticesConnections class
    /// </summary>
    /// <param name="graph">Graph from which connection matrix will be created</param>
    public VerticesConnections(Graph graph)     // Yes I know of DRY, but VS2022 kept giving me warnings if I didn't do it like this
    {
        this.graph = graph;

        List<indexT> unevenVertices = this.graph.GetUnevenVerticesIdxs();
        this.connectionMatrix = new Connection[unevenVertices.Count, unevenVertices.Count];

        this.indexTranslate = new Dictionary<indexT, indexT>();
        for (indexT thisIdx = 0; thisIdx < unevenVertices.Count; thisIdx++)
        {
            this.indexTranslate.Add(unevenVertices[thisIdx], thisIdx);
        }
    }

    /// <summary>
    /// Get string representation of the current state of a connection matrix. Mainly useful for debugging
    /// </summary>
    /// <returns>String representing a matrix</returns>
    public override string ToString()
    {
        var keys = this.indexTranslate.Keys.ToList();
        indexT len = this.connectionMatrix.GetLength(0);
        len += 5;
        
        string str = String.Concat(Enumerable.Repeat(" ", len)) + " |";
        foreach (var key in keys)
        {
            str += key.ToString().PadLeft(len) + " |";
        }

        for (indexT i=0; i<this.connectionMatrix.GetLength(0); i++)
        {
            str += "\n";
            str += String.Concat(Enumerable.Repeat("-", (len + 2) * (keys.Count + 1)));
            str += "\n";
            str += keys[i].ToString().PadLeft(len) + " |";

            for (indexT j = 0; j < this.connectionMatrix.GetLength(1); j++)
            {
                str += (this.connectionMatrix[i, j] == null ? "N/A" : this.connectionMatrix[i, j].ToString()).PadLeft(len) + " |";
            }
        }

        return str;
    }

    /// <summary>Calculate preceding vertices list and list of minimal costs using Dijkstra's algorithm.</summary>
    /// <param name="startVertex">The vertex to start the search from.</param>
    /// <returns>A tuple containing 2 lists: preceding vertices; minimal costs to vertices.</returns>
    private (indexT?[], edgeWeightT[]) CalcClassicDijkstra(indexT startVertex)
    {
        int verticesN = this.graph.GetVertexCount();

        // Array containing the minimum costs to reach each vertex from the starting vertex
        edgeWeightT[] minCostToVertex = new edgeWeightT[verticesN];
        // Array containing the preceding vertices on the path from the starting vertex
        indexT?[] precedingVertices = new indexT?[verticesN];

        // Working lists of vertices
        List<indexT> unvisitedVertices = new List<indexT>();
        List<indexT> visitedVertices = new List<indexT>();

        // Filing out verticesToProcess and minCostToVertex
        for (int i = 0; i < verticesN; i++)
        {
            minCostToVertex[i] = (i == startVertex) ? 0f : edgeWeightT.MaxValue;
            unvisitedVertices.Add(i);
        }

        while (unvisitedVertices.Count > 0)
        {
            indexT processedVertex = indexT.MaxValue;
            edgeWeightT tempMinCost = edgeWeightT.MaxValue;

            // Finding the vertex to process (with the minimum cost to reach from the starting vertex)
            foreach (indexT vertex in unvisitedVertices)
            {
                if (minCostToVertex[vertex] < tempMinCost)
                {
                    tempMinCost = minCostToVertex[vertex];
                    processedVertex = vertex;
                }
            }

            // Moving the current vertex to processed vertices
            if (unvisitedVertices.Remove(processedVertex))
            {
                visitedVertices.Add(processedVertex);
            }

            // Reviewing all neighbors of the relocated vertex
            for (indexT i = 0; i < graph[processedVertex].edgeList.Count(); i++)
            {
                Graph.Edge edge = graph[processedVertex].edgeList[i];
                indexT nextVertex = edge.targetIdx;
                edgeWeightT edgeCost = edge.weight;

                // Check if neighbour has not yet been processed
                if (unvisitedVertices.Contains(nextVertex))
                {
                    edgeWeightT CostToVertex = minCostToVertex[processedVertex] + edgeCost;

                    // Check the new cost and update if it is smaller than the old one
                    if (minCostToVertex[nextVertex] > CostToVertex)
                    {
                        minCostToVertex[nextVertex] = CostToVertex;
                        precedingVertices[nextVertex] = processedVertex;
                    }
                }
            }
        }

        return (precedingVertices, minCostToVertex);
    }

    /// <summary>Calculates path and cost using preceding vertices list and minimal costs to vertices list.</summary>
    /// <param name="precedingVertices">The list of vertex-predecessor on the path from starting vertex.</param>
    /// <param name="minCostToVertex">The list of minimal costs of reaching individual vertices from the starting vertex.</param>
    /// <param name="endVertex">The destination vertex of the search.</param>
    /// <returns>A tuple containing the shortest path and the distance.</returns>
    private (List<indexT>, edgeWeightT) GetPathAndCost(indexT?[] precedingVertices, edgeWeightT[] minCostToVertex, indexT endVertex)
    {
        indexT? tempVertex = endVertex;
        List<indexT> shortestPath = new List<indexT>();

        // Create the shortest path
        while (tempVertex != null)
        {
            shortestPath.Add(tempVertex.Value);
            tempVertex = precedingVertices[tempVertex.Value];
        }

        // Reverse the shortest path
        shortestPath.Reverse();

        // Get the path total cost
        edgeWeightT totalCost = minCostToVertex[endVertex];

        return (shortestPath, totalCost);
    }


    /// <summary>
    /// Function called by class' indexer. Gives connection between two vertices using connection matrix. Automatically calls Dijkstra algorithm when needed
    /// </summary>
    /// <param name="start">Index of starting vertex (from Graph class)</param>
    /// <param name="stop">Index of end vertex (from Graph class)</param>
    /// <returns>Connection (that is weight and path) from start to end vertex</returns>
    /// <exception cref="InvalidOperationException">Thrown if start vertex equals stop vertex</exception>
    /// <exception cref="IndexOutOfRangeException">Thrown if one of given vertices is not uneven, or does not exist in Graph class</exception>
    public Connection GetConnection(indexT start, indexT stop)
    {
        if (start == stop)
        {
            throw new InvalidOperationException("Can not get connection between the same vertex!");
        }
        else if (!this.indexTranslate.ContainsKey(start) || !this.indexTranslate.ContainsKey(stop))
        {
            throw new IndexOutOfRangeException("One of the given values does not exist in graph!");
        }
        else
        {
            //start = this.indexTranslate[start];
            //stop = this.indexTranslate[stop];

            if (this.connectionMatrix[this.indexTranslate[start], this.indexTranslate[stop]] == null)
            {
                (indexT?[] precedingVertices, edgeWeightT[] minCostToVertex) = CalcClassicDijkstra(start);

                List<indexT> endVertices = new List<indexT>();
                foreach (indexT j in this.indexTranslate.Keys)
                {
                    if (j != start && this.connectionMatrix[this.indexTranslate[start], this.indexTranslate[j]] == null)
                    {
                        endVertices.Add(j);
                    }
                }

                foreach (indexT endVertex in endVertices)
                {
                    (List<indexT> path, edgeWeightT weight) pAc = GetPathAndCost(precedingVertices, minCostToVertex, endVertex);
                    //List<indexT> path = new();
                    //foreach (indexT elem in pAc.path)
                    //{
                    //    path.Add(this.indexTranslate.FirstOrDefault(e => e.Value == elem).Key);
                    //}
                    List<indexT> pathReverse = Enumerable.Reverse(pAc.path).ToList();

                    this.connectionMatrix[this.indexTranslate[start], this.indexTranslate[endVertex]] = new Connection(pAc.weight, pAc.path);
                    this.connectionMatrix[this.indexTranslate[endVertex], this.indexTranslate[start]] = new Connection(pAc.weight, pathReverse);
                    
                    // Console.WriteLine(String.Join(" -> ", path));
                }
            }
            
            return this.connectionMatrix[this.indexTranslate[start], this.indexTranslate[stop]];
        }
    }

    /// <summary>
    /// Get indices of uneven vertices of graph
    /// </summary>
    /// <returns>List of uneven vertices' indices</returns>
    public List<indexT> GetUnevenVerticesIdxs()
    {
        return this.indexTranslate.Keys.ToList();
    }

    /// <summary>
    /// Smart way to get connection between two, uneven vertices. Uses connection matrix that automatically calls Dijkstra algorithm when needed
    /// </summary>
    /// <param name="start">Index of starting vertex (from Graph class)</param>
    /// <param name="stop">Index of end vertex (from Graph class)</param>
    /// <returns>Connection (that is weight and path) from start to end vertex</returns>
    /// <exception cref="InvalidOperationException">Thrown if start vertex equals stop vertex</exception>
    /// <exception cref="IndexOutOfRangeException">Thrown if one of given vertices is not uneven, or does not exist in Graph class</exception>
    public Connection this[indexT start, indexT stop]
    {
        get { return this.GetConnection(start, stop); }
    }
}

/*
 else if (!this.indexTranslate.ContainsKey(start) || !this.indexTranslate.ContainsKey(stop))
        {
            throw new IndexOutOfRangeException("One of the given values does not exist in graph!");
        }
 */