using System.Linq;

using indexT = System.Int32;


namespace SouvlakMVP;


public partial class GeneticAlgorithm
{
    /// <summary>
    /// Class representing a single Generation of genetic algorithm.
    /// </summary>
    public class Generation
    {
        /// <summary>
        /// Readonly field storing size of a population
        /// </summary>
        private readonly int populationSize;
        public int PopulationSize { get { return populationSize; } }
        
        /// <summary>
        /// Array storing population aka. array of genotypes
        /// Elements can be accessed via indexer
        /// </summary>
        private Genotype[] population;
        public Genotype this[indexT idx]
        {
            get 
            { 
                // Should be safe because genotype has it's own safety measures
                return this.population[idx]; 
            }
            set 
            {
                if (this.population[idx].Length == value.Length && this.population[idx].GetHashSet().SetEquals(value.GetHashSet()))
                {
                    this.population[idx] = value;
                }
                else
                {
                    throw new ArgumentException("New genotype does not contain all necessary vertices!");
                }
            }
        }
        
        /// <summary>
        /// Create initial population using a list of uneven vertices
        /// </summary>
        /// <param name="UnevenVertices">Vertices used to create initial population</param>
        /// <param name="populationSize">How big should the population be</param>
        public Generation(List<indexT> UnevenVertices, int populationSize)
        {
            this.populationSize = populationSize;
            this.population = new Genotype[populationSize];
            Random random = new Random();
            
            // loop for creating population_size numbers of genotypes in population
            for (int genotypeIdx = 0; genotypeIdx < populationSize; genotypeIdx++)
            {
                // Fisher-Yates algorithm for shuffling genotype members
                indexT[] currentGenotype = UnevenVertices.ToArray();
                int numOfVertices = currentGenotype.Length;
                while (numOfVertices > 1)
                {
                    int randomIdx = random.Next(numOfVertices--);
                    indexT vertexIdx = currentGenotype[randomIdx];
                    currentGenotype[randomIdx] = currentGenotype[numOfVertices];
                    currentGenotype[numOfVertices] = vertexIdx;
                }
                this.population[genotypeIdx] = new Genotype(currentGenotype);
            }
        }

        /// <summary>
        /// Create population from the arrray of genotypes
        /// </summary>
        /// <param name="population"></param>
        public Generation(Genotype[] population)
        {
            // @TODO Verify length
            this.populationSize = population.Length;
            this.population = population;
        }
    }
}
