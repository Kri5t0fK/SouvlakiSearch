using System.Linq;
using indexT = System.Int32;
using edgeWeightT = System.Single;

namespace SouvlakMVP;


public partial class GeneticAlgorithm
{
    private readonly Graph graph;
    private readonly VerticesConnections verticesConnections;
    private readonly int generationSize;
    private readonly Generation previousGeneration;
    private readonly Generation currentGeneration;
    
    private readonly Random random;

    public GeneticAlgorithm(Graph graph, int generationSize)
    {
        this.graph = graph;
        this.verticesConnections = new VerticesConnections(this.graph);
        this.generationSize = generationSize;
        this.previousGeneration = new Generation(this.verticesConnections.GetUnevenVerticesIdxs(), this.generationSize);
        this.currentGeneration = new Generation(new Genotype[this.generationSize]);

        this.random = new Random();
    }


    public static (Genotype child1, Genotype child2) Crossover(Genotype parent1, Genotype parent2)
    {
        // Get length and create children genes
        if (parent1.Length != parent2.Length)
        {
            throw new InvalidDataException("Genotypes have different lengths!");
        }
        int len = parent1.Length;
        indexT[] gene1 = Enumerable.Repeat<indexT>(-1, len).ToArray();
        indexT[] gene2 = Enumerable.Repeat<indexT>(-1, len).ToArray();

        // Get random crossover range
        Random rnd = new Random();
        indexT[] crossoverRange = { rnd.Next(0, len), rnd.Next(0, len) };
        Array.Sort(crossoverRange); // I know it has only two values, but one line functions look cool

        // Create exchange pairs
        Dictionary<indexT, indexT> gene1Exchange = new Dictionary<indexT, indexT>();
        Dictionary<indexT, indexT> gene2Exchange = new Dictionary<indexT, indexT>();

        for (int i = crossoverRange[0]; i < crossoverRange[1]; i++)
        {
            // Copy crossover range into children
            gene1[i] = parent1[i];
            gene2[i] = parent2[i];

            // Fill exchange pairs
            gene1Exchange.Add(key: gene1[i], value: gene2[i]);
            gene2Exchange.Add(key: gene2[i], value: gene1[i]);
        }

        // Fill remaining genes in children
        // If there is no conflict -> copy data from other parent
        // Otherwise use exchange pairs
        for (int i = 0; i < len; i++)
        {
            if (gene1[i] == -1)
            {
                indexT elem = parent2[i];
                int count = gene1Exchange.Count;    // Backup exit, because I do not trust "while" loops
                while (gene1.Contains(elem))        // Use exchange pairs in a loop, for as long as needed or until you iterate over all exchange pairs
                {
                    elem = gene1Exchange[elem];
                    if (count <= 0) { throw new IndexOutOfRangeException("Couldn't find exchange pair that works!"); };
                    count--;
                }
                gene1[i] = elem;
            }
            if (gene2[i] == -1)
            {
                indexT elem = parent1[i];
                int count = gene2Exchange.Count;    // Backup exit, because I do not trust "while" loops
                while (gene2.Contains(elem))        // Use exchange pairs in a loop, for as long as needed or until you iterate over all exchange pairs
                {
                    elem = gene2Exchange[elem];
                    if (count <= 0) { throw new IndexOutOfRangeException("Couldn't find exchange pair that works!"); };
                    count--;
                }
                gene2[i] = elem;
            }
        }

        // Return children
        return (new Genotype(gene1), new Genotype(gene2));
    }

    
}
