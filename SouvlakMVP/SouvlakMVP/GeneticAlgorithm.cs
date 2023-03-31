using System.Linq;
using indexT = System.Int32;
using edgeWeightT = System.Single;

namespace SouvlakMVP;


public partial class GeneticAlgorithm
{
    private readonly int generationSize;
    private readonly int selectionSize;
    private readonly int mutationChance;
    private readonly int maxIterations;
    private readonly int lastElementsToCheck;

    private Graph graph;
    private VerticesConnections verticesConnections;
    private Generation previousGeneration;
    private Generation currentGeneration;
    private List<edgeWeightT> bestWeightHistory;
    public List<edgeWeightT> BestWeightHistory
    {
        get { return bestWeightHistory; }
    }


    private readonly Random random;

    public GeneticAlgorithm(Graph graph, uint generationSize=20, uint selectionSize=10, uint mutationChance=33, uint maxIterations=1000, uint lastElementsToCheck=10)
    {
        if (generationSize % 2 == 1) { throw new ArgumentException("Generation size must be an even number!"); }
        if (selectionSize >= generationSize) { throw new ArgumentException("You can not select more individuals then there is in population!"); }
        if (selectionSize % 2 == 1) { throw new ArgumentException("Selection size must be an even number!"); }
        if (mutationChance > 100) { throw new ArgumentException("Mutation chance cannot be greater then 100%!"); }
        if (maxIterations <= lastElementsToCheck) { throw new ArgumentException("Can't check back more iterations then the allowed maximum number of iterations!"); }

        this.generationSize = (int) generationSize;
        this.selectionSize = (int) selectionSize;
        this.mutationChance = (int) mutationChance;
        this.maxIterations = (int) maxIterations;
        this.lastElementsToCheck = (int) lastElementsToCheck;
        
        this.graph = graph;
        this.verticesConnections = new VerticesConnections(this.graph);
        this.previousGeneration = new Generation(this.verticesConnections.GetUnevenVerticesIdxs(), this.generationSize);
        this.currentGeneration = new Generation(new Genotype[this.generationSize]);
        this.bestWeightHistory= new List<edgeWeightT>();

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

    /// <summary>
    /// Run this function in the loop to select "good enough" elements from previous generation and crossover + mutate them into current generation
    /// Aaand swap current and previous generation
    /// </summary>
    /// <param name="sortedIndicesAndWeights"></param>
    private void SelectionWithMutation((indexT index, edgeWeightT weight)[] sortedIndicesAndWeights)
    {
        for (int i = 0; i < this.generationSize; i += 2) 
        { 
            // Get two random and different indices, representing two "good enough" genotypes
            indexT firstParentIdx = sortedIndicesAndWeights[this.random.Next(0, this.selectionSize)].index;
            indexT secondParentIdx = sortedIndicesAndWeights[this.random.Next(0, this.selectionSize)].index;
            while (firstParentIdx == secondParentIdx) { secondParentIdx = sortedIndicesAndWeights[this.random.Next(0, this.selectionSize)].index; }

            // Crossover those two genotypes
            (this.currentGeneration[i], this.currentGeneration[i + 1]) = Crossover(this.previousGeneration[firstParentIdx], this.previousGeneration[secondParentIdx]);

            // Try to mutate new genotypes
            if (this.mutationChance > this.random.Next(0, 100)) { this.currentGeneration[i].Mutate(); }
            if (this.mutationChance > this.random.Next(0, 100)) { this.currentGeneration[i+1].Mutate(); }

            // Swap current and previous generation
            (this.currentGeneration, this.previousGeneration) = (this.previousGeneration, this.currentGeneration);
        }
    }

    /// <summary>
    /// Check whether algorithm should run or not
    /// </summary>
    /// <param name="iteration"></param>
    /// <returns></returns>
    private bool StopCondition(int iteration)
    {
        // Check wheter number of iterations didn't exceed maximum value
        if (iteration > this.maxIterations) { return true; }

        // Check if last "lastElementsToCheck" elements are the same
        var lastElems = this.bestWeightHistory.Skip(Math.Max(0, this.bestWeightHistory.Count() - this.lastElementsToCheck));
        var lastValue = this.bestWeightHistory[this.bestWeightHistory.Count()-1];
        if (lastElems.All(val => val == lastValue)) { return true; }

        return false;
    }

    public (edgeWeightT weight, Genotype genotype) MainLoop()
    {
        indexT bestIndex = 0;
        for (int i = 0; true; i++)  // I still prefer for loops over while loops
        {
            // Calculate fitness for previous population
            var indicesAndWeights = previousGeneration.GetIndicesAndWeights(this.verticesConnections);

            // Add best (smallest) weight to list and update best index
            this.bestWeightHistory.Add(indicesAndWeights[0].weight);
            bestIndex = indicesAndWeights[0].index;

            // If conditions fulfilled -> Stop
            if (this.StopCondition(i)) { break; }

            // Selection, crossover, mutation and population swap
            this.SelectionWithMutation(indicesAndWeights);
        }

        return (this.bestWeightHistory[this.bestWeightHistory.Count()-1], new Genotype(this.previousGeneration[bestIndex].UnevenVerticesIdxs));
    }
}
