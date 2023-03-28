using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using SouvlakMVP;
using System.Numerics;
using static SouvlakMVP.GeneticAlgorithm;
using static SouvlakMVP.GeneticAlgorithm.Genotype;

namespace SouvlakMVPTest
{
    [TestClass]
    public class TestGenotype
    {
        [TestMethod]
        public void OddVertices_getVertices()
        {
            Graph.Vertex vertex1 = new Graph.Vertex(new Vector2(0, 1));
            Graph.Vertex vertex2 = new Graph.Vertex(new Vector2(1, 2));
            List<Graph.Vertex> testOddVertices = new List<Graph.Vertex> { vertex1, vertex2 };

            Genotype testGenotype = new Genotype(testOddVertices);
            Assert.AreEqual(testGenotype.OddVertices[0], testOddVertices[0]);
            Assert.AreEqual(testGenotype.OddVertices[1], testOddVertices[1]);
        }

        [TestMethod]
        [ExpectedException(typeof(NonUniqueVerticesException),
            "Duplicate edges with different weights are not allowed!")]
        public void Genotype_not_unique_vertices()
        {
            Graph.Vertex vertex1 = new Graph.Vertex(new Vector2(0, 0));
            Graph.Vertex vertex2 = new Graph.Vertex(new Vector2(0, 0));
            List<Graph.Vertex> testOddVertices = new List<Graph.Vertex> { vertex1, vertex2 };
            Genotype testGenotype = new Genotype(testOddVertices);
        }

        [TestMethod]
        [ExpectedException(typeof(NonEvenNumberOfVerticesException),
            "Genotype must have EVEN (ont odd) number of vertices!")]
        public void Genotype_odd_number_of_vertices()
        {
            Graph.Vertex vertex1 = new Graph.Vertex(new Vector2(1, 2));
            Graph.Vertex vertex2 = new Graph.Vertex(new Vector2(2, 3));
            Graph.Vertex vertex3 = new Graph.Vertex(new Vector2(3, 4));
            List<Graph.Vertex> testOddVertices = new List<Graph.Vertex> { vertex1, vertex2, vertex3 };
            Genotype testGenotype = new Genotype(testOddVertices);
        }

        [TestMethod]
        public void GetPairs_get_vertex_pairs()
        {
            Graph.Vertex vertex1 = new Graph.Vertex(new Vector2(0, 1));
            Graph.Vertex vertex2 = new Graph.Vertex(new Vector2(1, 2));
            Graph.Vertex vertex3 = new Graph.Vertex(new Vector2(2, 3));
            Graph.Vertex vertex4 = new Graph.Vertex(new Vector2(3, 4));
            List<Graph.Vertex> testOddVertices = new List<Graph.Vertex> { vertex1, vertex2, vertex3, vertex4 };

            Genotype testGenotype = new Genotype(testOddVertices);

            List<(Graph.Vertex, Graph.Vertex)> testPairsList = new List<(Graph.Vertex, Graph.Vertex)> { (vertex1, vertex2), (vertex3, vertex4) };
            List<(Graph.Vertex, Graph.Vertex)> testGenotypePairsList = testGenotype.GetPairs();
            Assert.AreEqual(testGenotypePairsList[0], testPairsList[0]);
            Assert.AreEqual(testGenotypePairsList[1], testPairsList[1]);
        }

        [TestMethod]
        public void Size_get_size()
        {
            Graph.Vertex vertex1 = new Graph.Vertex(new Vector2(0, 1));
            Graph.Vertex vertex2 = new Graph.Vertex(new Vector2(1, 2));
            Graph.Vertex vertex3 = new Graph.Vertex(new Vector2(2, 3));
            Graph.Vertex vertex4 = new Graph.Vertex(new Vector2(3, 4));
            List<Graph.Vertex> testOddVertices = new List<Graph.Vertex> { vertex1, vertex2, vertex3, vertex4 };

            Genotype testGenotype = new Genotype(testOddVertices);
            Assert.AreEqual(testGenotype.Size, 4);
        }
    }
}