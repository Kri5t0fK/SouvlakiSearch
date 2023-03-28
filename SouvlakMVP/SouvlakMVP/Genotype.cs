using System;
using System.Collections.Generic;

using genotypeSizeT = System.Int32;

namespace SouvlakMVP;

/// <summary>
/// Class representing a single genotype of genetic algorithm.
/// </summary>
public class Genotype
{
    public class NonUniqueVerticesException : Exception
    {
        public NonUniqueVerticesException() : base("Duplicate edges with different weights are not allowed!") { }
        public NonUniqueVerticesException(string message) : base(message) { }
    }

    public class NonEvenNumberOfVerticesException : Exception
    {
        public NonEvenNumberOfVerticesException() : base("Genotype must have EVEN (ont odd) number of vertices!") { }
        public NonEvenNumberOfVerticesException(string message) : base(message) { }
    }

    /// <summary>
    /// Genotype: a list of even number of "odd vertices".
    /// Odd vertices - vertices with odd number of neighbours.
    /// </summary>
    private readonly List<Graph.Vertex> _oddVertices;
    public List<Graph.Vertex> OddVertices { get { return new List<Graph.Vertex>(this._oddVertices); } }
    // Read-only Indexer for accesing the list
    public Graph.Vertex this[int i]
    {
        get { return this._oddVertices[i]; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_oddVertices"></param>
    /// <exception cref="ArgumentException"></exception>
    public Genotype(List<Graph.Vertex> _oddVertices)
    {
        // Validity checks
        if(!(_oddVertices.Distinct().Count() == _oddVertices.Count()))
        {
            throw new NonUniqueVerticesException();
        }
        if(!(_oddVertices.Count() % 2 == 0))
        {
            throw new NonEvenNumberOfVerticesException();
        }

        this._oddVertices = _oddVertices;
    }

    public List<(Graph.Vertex, Graph.Vertex)> GetPairs()
    {
        // Define output list
        List<(Graph.Vertex,Graph.Vertex)> output = new();

        // Get number of pairs in vertex list, list count is always even
        int pairsCount = this.Size/2;
        for(int pairID=0; pairID < pairsCount; pairID++)
        {
            (Graph.Vertex, Graph.Vertex) pair = (this[pairID], this[pairID+1]); 
            output.Add(pair);
        }
        // Return list of pairs
        return output;
    }

    public genotypeSizeT Size
    {
        get { return this._oddVertices.Count; }
    }
}
