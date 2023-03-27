using Microsoft.VisualStudio.TestTools.UnitTesting;
using SouvlakMVP;
using System.Numerics;

namespace SouvlakMVPTest
{
    [TestClass]   
    
    public class TestVertex
    {

        [TestMethod]
        public void AddEdge_withNewEdge()
        {

            Graph testGraph = new Graph();
            testGraph.AddVertex(new Graph.Vertex(new Vector2(0, 0)));
            testGraph.AddVertex(new Graph.Vertex(new Vector2(1, 0)));

            // add edge between two above vertices
            testGraph[0].AddEdge(1, 3f, 1);
            Graph.Edge expected = new Graph.Edge(1, 3f, 1);

            Assert.AreEqual(testGraph.GetEdge(0, 0), expected);
        }

        [TestMethod]
        public void AddEdge_withTheSameEdgeAndWeight()
        {

            Graph testGraph = new Graph();
            testGraph.AddVertex(new Graph.Vertex(new Vector2(0, 0)));
            testGraph.AddVertex(new Graph.Vertex(new Vector2(1, 0)));

            // add edge between two above vertices
            testGraph.AddEdge(0, 1, 3f);
            testGraph.AddEdge(0, 1, 3f);
            Graph.Edge expected = new Graph.Edge(1, 3, 2);

            Assert.AreEqual(testGraph.GetEdge(0, 0), expected);
        }

        [TestMethod]
        public void AddEdge_withTheSameEdgeDifferentWeight()
        {

            Graph testGraph = new Graph();
            testGraph.AddVertex(new Graph.Vertex(new Vector2(0, 0)));
            testGraph.AddVertex(new Graph.Vertex(new Vector2(1, 0)));

            // add edge between two above vertices
            testGraph.AddEdge(0, 1, 3f);
            Action badAddEdge = () => testGraph.AddEdge(0, 1, 2f);
            Assert.ThrowsException<Graph.DuplicateEdgeException>(badAddEdge);
;
        }

        [TestMethod]
        public void ContainsEdge_withEdgeBetweenVertices()
        {

            Graph testGraph = new Graph();
            testGraph.AddVertex(new Graph.Vertex(new Vector2(0, 0)));
            testGraph.AddVertex(new Graph.Vertex(new Vector2(1, 0)));

            // add edge between two above vertices
            testGraph.AddEdge(0, 1, 3f);

            Assert.AreEqual(testGraph.ContainsEdge(0, 1), true);
        }

        [TestMethod]
        public void ContainsEdge_withoutEdgeBetweenVertices()
        {

            Graph testGraph = new Graph();
            testGraph.AddVertex(new Graph.Vertex(new Vector2(0, 0)));
            testGraph.AddVertex(new Graph.Vertex(new Vector2(1, 0)));
            testGraph.AddVertex(new Graph.Vertex(new Vector2(2, 0)));

            // add edge between two above vertices
            testGraph.AddEdge(0, 1, 3f);

            // graph doesnt contain edge from 0 to 2 so they should not be equal
            Assert.AreNotEqual(testGraph.ContainsEdge(0, 2), true);
        }
    }
}
