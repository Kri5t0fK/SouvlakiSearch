using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static SouvlakMVP.Genotype;

namespace SouvlakMVP;


public partial class GeneticAlgorithm
{
    /// <summary>
    /// Class representing a single Generation of genetic algorithm.
    /// </summary>
    public class Generation
    {
        // field rng is used for shuffling in generating population
        private static Random rng = new Random();
        private int populationSize;
        private Genotype[] population;

        /// <exception cref="ArgumentException"></exception>
        public Generation(List<Graph.Vertex> init_population, int size)
        {
            if(size > 0)
            {
                this.populationSize = size;
            }
            else
            {
                throw new ArgumentOutOfRangeException("Population size can't be negative!");
            }
            this.population = new Genotype[populationSize];
            GeneratePopulation(init_population, this.populationSize);
        }

        public Generation(Genotype[] population)
        {
            // @TODO Verify length
            this.populationSize = population.Length;
            this.population = population;
        }

        /// <summary>
        /// Method used in Generator constructor. Used for creating population.
        /// </summary>
        /// <param name="init_population">Initial genotype (list of Vertex) used for creating population.</param>
        /// <param name="population_size">Specifies amount of subsets created in current generation.</param>
        private void GeneratePopulation(List<Graph.Vertex> init_population, int population_size)
        {
            // loop for creating population_size numbers of genotypes in population
            for(uint genotypeIdx = 0; genotypeIdx < population_size; genotypeIdx++)
            {
                // Fisher-Yates algorithm for shuffling genotype members
                List<Graph.Vertex> curr_genotype = new List<Graph.Vertex>(init_population);
                int no_of_vertices = curr_genotype.Count;
                while (no_of_vertices > 1)
                {
                    int rand_vertex = Generation.rng.Next(no_of_vertices--);
                    Graph.Vertex value = curr_genotype[rand_vertex];
                    curr_genotype[rand_vertex] = curr_genotype[no_of_vertices];
                    curr_genotype[no_of_vertices] = value;
                }
                this.population[genotypeIdx] = new Genotype(curr_genotype);
            }
        }
    }
}
