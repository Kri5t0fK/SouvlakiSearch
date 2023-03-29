using Microsoft.VisualStudio.TestTools.UnitTesting;
using SouvlakMVP;
using System.Numerics;

namespace SouvlakMVPTest
{
    [TestClass]
    public class TestGeneration
    {
        [TestMethod]
        public void GenerationConstructor_ExceptionThrow()
        {
            // Example map (graph) with 6 intersections (vertices) 
            Graph graph = new Graph();
            // Intersections from 0 to 5 
            graph.AddVertex(new Graph.Vertex(new Vector2(0, 0)));
            graph.AddVertex(new Graph.Vertex(new Vector2(1, 0)));
            graph.AddVertex(new Graph.Vertex(new Vector2(0, 1)));
            graph.AddVertex(new Graph.Vertex(new Vector2(1, 1)));
            graph.AddVertex(new Graph.Vertex(new Vector2(2, 1)));
            graph.AddVertex(new Graph.Vertex(new Vector2(1, 2)));
            // 9 roads and their distances 
            graph.AddEdge(0, 1, 3f);
            graph.AddEdge(0, 5, 6f);
            graph.AddEdge(0, 4, 3f);
            graph.AddEdge(1, 2, 1f);
            graph.AddEdge(1, 3, 3f);
            graph.AddEdge(2, 3, 3f);
            graph.AddEdge(2, 5, 1f);
            graph.AddEdge(3, 5, 1f);
            graph.AddEdge(4, 5, 2f);

            List<Graph.Vertex> vertices = graph.GetUnevenVertices();

            Action badPopulationSize = () => new GeneticAlgorithm.Generation(vertices, -4);
            Assert.ThrowsException<ArgumentOutOfRangeException>(badPopulationSize);
        }

        [TestMethod]
        public void GenerationConstructor_CreatingObject()
        {
            // Example map (graph) with 6 intersections (vertices) 
            Graph graph = new Graph();
            // Intersections from 0 to 5 
            graph.AddVertex(new Graph.Vertex(new Vector2(0, 0)));
            graph.AddVertex(new Graph.Vertex(new Vector2(1, 0)));
            graph.AddVertex(new Graph.Vertex(new Vector2(0, 1)));
            graph.AddVertex(new Graph.Vertex(new Vector2(1, 1)));
            graph.AddVertex(new Graph.Vertex(new Vector2(2, 1)));
            graph.AddVertex(new Graph.Vertex(new Vector2(1, 2)));
            // 9 roads and their distances 
            graph.AddEdge(0, 1, 3f);
            graph.AddEdge(0, 5, 6f);
            graph.AddEdge(0, 4, 3f);
            graph.AddEdge(1, 2, 1f);
            graph.AddEdge(1, 3, 3f);
            graph.AddEdge(2, 3, 3f);
            graph.AddEdge(2, 5, 1f);
            graph.AddEdge(3, 5, 1f);
            graph.AddEdge(4, 5, 2f);

            List<Graph.Vertex> vertices = graph.GetUnevenVertices();
            GeneticAlgorithm.Generation testgen = new GeneticAlgorithm.Generation(vertices, 6);
            Assert.IsInstanceOfType(testgen, typeof(GeneticAlgorithm.Generation));
        }
    }
}
