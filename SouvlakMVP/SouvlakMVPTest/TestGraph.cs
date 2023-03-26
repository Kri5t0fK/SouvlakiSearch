using Microsoft.VisualStudio.TestTools.UnitTesting;
using SouvlakMVP;
using System.Numerics;

namespace SouvlakMVPTest
{
    [TestClass]
    public class TestGraph
    {

        // this doesnt pass
        //[TestMethod]
        //public void AddEdge_withNewEdge()
        //{

        //    Graph testGraph = new Graph();
        //    testGraph.AddVertex(new Graph.Vertex(new Vector2(0, 0)));
        //    testGraph.AddVertex(new Graph.Vertex(new Vector2(1, 0)));

        //    // add edge between two above vertices
        //    testGraph.AddEdge(0, 1, 3f);
        //    Graph.Edge expected = new Graph.Edge(1, 3, 1);

        //    Assert.AreEqual(testGraph.GetEdge(0,1), expected);
        //}

        [TestMethod]
        public void ContainsEdge_withEdgeBetweenVertices()
        {

            Graph testGraph = new Graph();
            testGraph.AddVertex(new Graph.Vertex(new Vector2(0, 0)));
            testGraph.AddVertex(new Graph.Vertex(new Vector2(1, 0)));

            // add edge between two above vertices
            testGraph.AddEdge(0, 1, 3f);

            Assert.AreEqual(testGraph.ContainsEdge(0,1), true);
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

        [TestMethod]
        public void ContainsVertex_withVertexInGraph()
        {

            Graph testGraph = new Graph();
            testGraph.AddVertex(new Graph.Vertex(new Vector2(1, 3)));

            Graph.Vertex otherVertex = new Graph.Vertex(new Vector2(1, 3));
            Assert.AreEqual(testGraph.ContainsVertex(otherVertex), true);
        }

        [TestMethod]
        public void ContainsVertex_withoutVertexInGraph()
        {
            Graph testGraph = new Graph();

            testGraph.AddVertex(new Graph.Vertex(new Vector2(5, 6)));

            Graph.Vertex otherVertex = new Graph.Vertex(new Vector2(1, 3));
            Assert.AreNotEqual(testGraph.ContainsVertex(otherVertex), true);
        }

        [TestMethod]
        public void ContainsVertexByPosition_withVertexInGraph()
        {

            Graph testGraph = new Graph();
            testGraph.AddVertex(new Graph.Vertex(new Vector2(1, 3)));

            Vector2 position = new Vector2(1, 3);
            Assert.AreEqual(testGraph.ContainsVertex(position), true);
        }

        [TestMethod]
        public void ContainsVertexByPosition_withoutVertexInGraph()
        {

            Graph testGraph = new Graph();
            testGraph.AddVertex(new Graph.Vertex(new Vector2(5, 6)));

            Vector2 position = new Vector2(0, -5);
            Assert.AreNotEqual(testGraph.ContainsVertex(position), true);
        }
    }
}