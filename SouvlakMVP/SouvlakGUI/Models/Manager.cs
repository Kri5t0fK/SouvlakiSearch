using System.Reflection;

namespace SouvlakGUI.Models;


public class Manager
{
#nullable enable
    public List<string> ExampleGraphs { get; set; } = new List<string>() { "test", "graphV4E4", "graphV4E5", "graphV6E9", "graphV10E15", "graphV77E123" };

    public Graph? SelectedGraph { get; set; } = null;
    public GeneticAlgorithm? Algorithm { get; set; } = null;


    public void ReloadGraph(Graph? graph)
    {
        this.SelectedGraph = graph;
        this.Algorithm = null;
    }


    public int d;


    public void Calculate()
    {
        this.Algorithm = new GeneticAlgorithm(this.SelectedGraph);
        this.Algorithm.MainLoop();
    }

    public Manager()
    {

    }


}
