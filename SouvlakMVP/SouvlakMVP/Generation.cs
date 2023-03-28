using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static SouvlakMVP.Genotype;

namespace SouvlakMVP;

/// <summary>
/// Class representing a single Generation of genetic algorithm.
/// </summary>
public class Generation
{
    private int populationSize;
    private Genotype[] population;
    public Generation(int size)
    {
        this.populationSize = size;
        this.population = new Genotype[populationSize];

    }

    public Generation(Genotype[] population)
    {
        // @TODO Verify length
        this.populationSize = population.Length;
        this.population = population;
    }

    public void GeneratePopulation()
    {
        //     @TODO  Implement Initialization
    }
}