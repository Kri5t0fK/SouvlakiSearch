using System.ComponentModel;

namespace SouvlakGUI.Models;


public class Manager : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #nullable enable
    public List<string> ExampleGraphs { get; set; } = new List<string>() { "Vertices4", 
                                                                           "Vertices6", 
                                                                           "Vertices10", 
                                                                           "Vertices40",
                                                                           "Vertices50",
                                                                           "Vertices75",
                                                                           "Vertices100"
                                                                         };

    public Graph? SelectedGraph { get; set; } = null;
    public Graph? UpdatedGraph { get; set; } = null;
    public GeneticAlgorithm? Algorithm { get; set; } = null;


    private int _iterations;
    public int Iterations { get; set; }
    public bool IterationsValidate()
    {
        if (Iterations > 0 && Iterations > this._stopConditionSize)
        {
            this._iterations = this.Iterations;
            return true;
        }
        else
        {
            this.Iterations = this._iterations;
            OnPropertyChanged(nameof(Iterations));
            return false;
        }
    }

    private int _generationSize;
    public int GenerationSize { get; set; }
    public bool GenerationSizeValidate()
    {
        if (GenerationSize % 2 == 0 && GenerationSize >= 2 && GenerationSize > this._selectionSize)
        {
            this._generationSize = this.GenerationSize;
            return true;
        }
        else
        {
            this.GenerationSize = this._generationSize;
            OnPropertyChanged(nameof(GenerationSize));
            return false;
        }
    }

    private int _selectionSize;
    public int SelectionSize { get; set; }
    public bool SelectionSizeValidate()
    {
        if (SelectionSize % 2 == 0 && SelectionSize >= 2 && SelectionSize < this._generationSize)
        {
            this._selectionSize = this.SelectionSize;
            return true;
        }
        else
        {
            this.SelectionSize = this._selectionSize;
            OnPropertyChanged(nameof(SelectionSize));
            return false;
        }
    }

    private int _mutationChance;
    public int MutationChance{ get; set; }
    public bool MutationChanceValidate()
    {
        if (MutationChance >= 0 && MutationChance <= 100)
        {
            this._mutationChance = this.MutationChance;
            return true;
        }
        else
        {
            this.MutationChance = this._mutationChance;
            OnPropertyChanged(nameof(MutationChance));
            return false;
        }
    }

    private int _stopConditionSize;
    public int StopConditionSize { get; set; }
    public bool StopConditionSizeValidate()
    {
        if (StopConditionSize >= 0 && StopConditionSize < this._iterations)
        {
            this._stopConditionSize = this.StopConditionSize;
            return true;
        }
        else
        {
            this.StopConditionSize = this._stopConditionSize;
            OnPropertyChanged(nameof(StopConditionSize));
            return false;
        }
    }


    public double? BestGenotypeWeight { get; set; } = null;
    public GeneticAlgorithm.Genotype? BestGenotype { get; set; } = null;
    public double? TotalCost { get; set; } = null;
    public List<int>? EulerCycle { get; set; } = null;


    public void ReloadGraph(Graph? graph)
    {
        this.BestGenotypeWeight = null;
        this.BestGenotype = null;
        this.TotalCost = null;
        this.EulerCycle = null;
        this.SelectedGraph = graph;
        this.UpdatedGraph = null;
        this.Algorithm = null;

        OnPropertyChanged(nameof(SelectedGraph));
        OnPropertyChanged(nameof(BestGenotypeWeight));
        OnPropertyChanged(nameof(TotalCost));
        OnPropertyChanged(nameof(EulerCycle));
        OnPropertyChanged(nameof(Algorithm));
    }

    //public GeneticAlgorithm.SelectionType selectionType { get; set; } = GeneticAlgorithm.SelectionType.NBest;
    //public GeneticAlgorithm.CrossoverType crossoverType { get; set; } = GeneticAlgorithm.CrossoverType.TwoPoint;
    public bool UseRankSelection { get; set; } = false;
    public bool UseOnePointCrossover { get; set; } = false;


    public void Calculate()
    {
        if (this.SelectedGraph == null) { throw new InvalidDataException("Selected graph can not be a null!"); };
        this.Algorithm = new GeneticAlgorithm(this.SelectedGraph, (uint) this._generationSize, (uint) this._selectionSize, (uint) this._mutationChance, (uint) this._iterations, (uint) this._stopConditionSize);
        (this.BestGenotypeWeight, this.BestGenotype) = this.Algorithm.MainLoop();
        this.UpdatedGraph = this.Algorithm.GetUpdatedGraph(this.BestGenotype);

        (this.EulerCycle, this.TotalCost) = Euler.FindEulerCycle(this.UpdatedGraph, copyGraph: true);

        OnPropertyChanged(nameof(BestGenotypeWeight));
        OnPropertyChanged(nameof(TotalCost));
        OnPropertyChanged(nameof(EulerCycle));
        OnPropertyChanged(nameof(Algorithm));
    }
    

    public Manager()
    {
        this._iterations = 1000;
        this.Iterations = this._iterations;

        this._generationSize = 20;
        this.GenerationSize = this._generationSize;

        this._selectionSize = 10;
        this.SelectionSize = this._selectionSize;

        this._mutationChance = 50;
        this.MutationChance = this._mutationChance;

        this._stopConditionSize = 10;
        this.StopConditionSize = this._stopConditionSize;
    }


}
