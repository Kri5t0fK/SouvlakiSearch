using Microsoft.VisualStudio.TestTools.UnitTesting;
using SouvlakMVP;
using System.Numerics;

namespace SouvlakMVPTest
{
    [TestClass]
    public class TestGraph
    {
        [TestMethod]
        public void GetVertexCount_zeroVertices()
        {

            Graph testGraph = new Graph();
            int expected = 0;
            Assert.AreEqual(testGraph.GetVertexCount(), expected);
        }

        [TestMethod]
        public void GetVertexCount_aboveZeroVertices()
        {

            Graph testGraph = new Graph();
            testGraph.AddVertex(new Graph.Vertex(new Vector2(-7, 0)));
            testGraph.AddVertex(new Graph.Vertex(new Vector2(1, 0.5f)));

            int expected = 2;
            Assert.AreEqual(testGraph.GetVertexCount(), expected);
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