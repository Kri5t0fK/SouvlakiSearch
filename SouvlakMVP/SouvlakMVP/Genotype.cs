using System;
using System.Collections.Generic;

using genotypeSizeT = System.Int32;
using indexT = System.Int32;


namespace SouvlakMVP;


public partial class GeneticAlgorithm
{
    /// <summary>
    /// Class representing a single genotype of genetic algorithm.
    /// </summary>
    public class Genotype
    {
        /// <summary>
        /// Exception raised when there are duplicate vertices in input list for Genotype.
        /// </summary>
        public class NonUniqueVerticesException : Exception
        {
            public NonUniqueVerticesException() : base("Duplicate edges with different weights are not allowed!") { }
            public NonUniqueVerticesException(string message) : base(message) { }
        }

        /// <summary>
        /// Exception raised when the number of Vertices in input list is odd.
        /// </summary>
        public class NonEvenNumberOfVerticesException : Exception
        {
            public NonEvenNumberOfVerticesException() : base("Genotype must have EVEN (ont odd) number of vertices!") { }
            public NonEvenNumberOfVerticesException(string message) : base(message) { }
        }

        /// <summary>
        /// Genotype: a list of even number of "uneven vertices" indexes.
        /// Odd vertices - vertices with odd number of neighbours.
        /// </summary>
        private readonly indexT[] unevenVerticesIdxs;

        /// <summary>
        /// Getter for a copy of Vertices indices list of Genotype.
        /// </summary>
        public indexT[] UnevenVerticesIdxs { get {
                indexT[] output = new indexT[unevenVerticesIdxs.Length];
                Array.Copy(this.unevenVerticesIdxs, output, unevenVerticesIdxs.Length);
                return output; 
            } }

        // Read-only Indexer for accesing the list
        public indexT this[int i]
        {
            get { return this.unevenVerticesIdxs[i]; }
        }

        /// <summary>
        /// Genotype class constructor. Eeceives an array of Vertices indices that represent the Genotype.
        /// </summary>
        /// <param name="unevenVerticesIdxs"></param>
        /// <exception cref="ArgumentException"></exception>
        public Genotype(indexT[] unevenVerticesIdxs)
        {
            // Validity checks
            if (!(unevenVerticesIdxs.Distinct().Count() == unevenVerticesIdxs.Length))
            {
                throw new NonUniqueVerticesException();
            }
            if (!(unevenVerticesIdxs.Length % 2 == 0))
            {
                throw new NonEvenNumberOfVerticesException();
            }

            this.unevenVerticesIdxs = new indexT[unevenVerticesIdxs.Length];
            Array.Copy(unevenVerticesIdxs, this.unevenVerticesIdxs, unevenVerticesIdxs.Length);
        }

        /// <summary>
        /// Getter for Genotype indices array size.
        /// </summary>
        public genotypeSizeT Length
        {
            get { return this.unevenVerticesIdxs.Length; }
        }

        /// <summary>
        /// Returns array of tuple pairs of Vertices indices of Genotype.
        /// </summary>
        public (indexT start, indexT stop)[] GetPairs()
        {
            // Get number of pairs in indices array, array length is always even
            int pairsCount = this.Length / 2;

            // Define output array
            (indexT start, indexT stop)[] output = new (indexT start, indexT stop)[pairsCount];

            for (int pairID = 0; pairID < pairsCount; pairID++)
            {
                output[pairID] = (this[2 * pairID], this[2 * pairID + 1]);
            }
            // Return list of pairs
            return output;
        }

        /// <summary>
        /// Check whether genotype contains a value
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <param name="start">Optional: from where to start search</param>
        /// <param name="stop">Optional: where to end search</param>
        /// <returns></returns>
        public bool Contains(indexT value, int? start = null, int? stop = null)
        {
            if (start == null)
            {
                start = 0;
            }
            if (stop == null)
            {
                stop = this.Length;
            }
            for (int i=start.Value; i<stop.Value; i++)
            {
                if (this.unevenVerticesIdxs[i] == value) { return true; }
            }

            return false;
        }

        public override string ToString()
        {
            string str = "[ ";
            foreach (indexT idx in this.unevenVerticesIdxs) 
            {
                str += idx.ToString() + " ";
            }
            return str + "]";
        }

    }
}