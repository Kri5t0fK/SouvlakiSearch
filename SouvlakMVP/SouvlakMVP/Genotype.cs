using System;
using System.Collections.Generic;

using genotypeSizeT = System.Int32;

namespace SouvlakMVP;

/// <summary>
/// Class representing a single genotype of genetic algorithm.
/// </summary>
public class Genotype
{
    // Declare two new Exceptions for Genotype constructing:
    // NonUniqueVerticesException: raised when given list of Vertices has duplicates
    public class NonUniqueVerticesException : Exception
    {
        public NonUniqueVerticesException() : base("Duplicate edges with different weights are not allowed!") { }
        public NonUniqueVerticesException(string message) : base(message) { }
    }

    // NonEvenNumberOfVerticesException: raised when given list has odd number of Vertices (which is impossible because graph theory)
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
    
    // Getter for field list of Veritces
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

        // If Valididty checks pass, create new Genotype instance
        this._oddVertices = _oddVertices;
    }

    public List<(Graph.Vertex, Graph.Vertex)> GetPairs()
    {
        // Define output list
        List<(Graph.Vertex,Graph.Vertex)> output = new();

        // Get number of pairs in vertex list, list count is always even
        int pairsCount = this.Size/2;

        // Iterate thorugh list of Vertices, create tuple pairs and add them to output list
        for(int pairID=0; pairID < pairsCount; pairID++)
        {
            (Graph.Vertex, Graph.Vertex) pair = (this[2*pairID], this[2*pairID+1]); 
            output.Add(pair);
        }

        // Return list of pairs
        return output;
    }

    // Method for getting Genotype size (numeber of Vertices in private list)
    public genotypeSizeT Size
    {
        get { return this._oddVertices.Count; }
    }
}
