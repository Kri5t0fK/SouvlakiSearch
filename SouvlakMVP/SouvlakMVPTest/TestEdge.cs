using Microsoft.VisualStudio.TestTools.UnitTesting;
using SouvlakMVP;
using System.Numerics;

namespace SouvlakMVPTest
{
    [TestClass]

    public class TestEdge
    {
        [TestMethod]
        public void EdgeEqualTo_sameWeights()
        {

            Graph testGraph = new Graph();
            testGraph.AddVertex(new Graph.Vertex(new Vector2(0, 0)));
            testGraph.AddVertex(new Graph.Vertex(new Vector2(1, 0)));

            // add edge between two above vertices
            testGraph.AddEdge(0, 1, 3f);

            Assert.AreEqual(testGraph.ContainsEdge(0, 1), true);
        }
    }
}
