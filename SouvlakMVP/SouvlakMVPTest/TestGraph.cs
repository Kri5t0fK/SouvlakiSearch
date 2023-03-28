using Microsoft.VisualStudio.TestTools.UnitTesting;
using SouvlakMVP;
using System.Numerics;
using indexT = System.Int32;

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

        [TestMethod]
        public void GetVertexIdx_withVertexInGraph()
        {
            Graph.Vertex vertex = new Graph.Vertex(new Vector2(1, 1));
            Graph testGraph = new Graph(vertex);
            indexT vertex_idx = testGraph.GetVertexIdx(vertex);


            indexT expected = 0;

            Assert.AreEqual(expected, vertex_idx);
        }

        [TestMethod]
        public void GetVertexIdx_withoutVertexInGraph()
        {
            // in my opinion it should throw exception 
            Graph testGraph = new Graph();
            indexT vertex_id = testGraph.GetVertexIdx(new Graph.Vertex(new Vector2(1, 1)));

            indexT expected = -1;

            Assert.AreEqual(expected, vertex_id);

        }

        [TestMethod]
        public void GetVertexIdxByPosition_withVertexInGraph()
        {
            Graph.Vertex[] vertices_position = { new Graph.Vertex(new Vector2(1, 1)),
                                                     new Graph.Vertex(new Vector2(2, 2))};
            List<Graph.Vertex> vertices = new List<Graph.Vertex>(vertices_position);
            Graph testGraph = new Graph(vertices);

            indexT vertex_idx = testGraph.GetVertexIdx(new Vector2(2, 2));

            indexT expected = 1;

            Assert.AreEqual(expected, vertex_idx);

        }

        public void GetVertexIdxByPosition_withoutVertexInGraph()
        {
            // in my opinion it should throw exception 
            Graph testGraph = new Graph();
            indexT vertex_id = testGraph.GetVertexIdx(new Vector2(1, 1));

            indexT expected = -1;

            Assert.AreEqual(expected, vertex_id);

        }

        [TestMethod]
        public void ToString_EmptyGraph()
        {
            Graph graph = new Graph();
            string expected = string.Empty;

            Assert.AreEqual(expected, graph.ToString());

        }

    }
}