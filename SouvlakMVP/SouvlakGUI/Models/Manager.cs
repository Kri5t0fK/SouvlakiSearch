using System.Reflection;

namespace SouvlakGUI.Models;


public class Manager
{
    #nullable enable
    public List<string> ExampleGraphs { get; set; } = new List<string>() { "person", "test", "graphV4E4", "graphV4E5", "graphV6E9", "graphV10E15" };

    public Graph? SelectedGraph { get; set; } = null;
    public GeneticAlgorithm? Algorithm { get; set; } = null;


    public void ReloadGraph(Graph? graph)
    {
        this.SelectedGraph = graph;
        this.Algorithm = null;
    }

    public Manager()
    {

    }


}
