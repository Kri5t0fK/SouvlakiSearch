using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using SouvlakMVP;
using System.Numerics;
using static SouvlakMVP.GeneticAlgorithm;
using static SouvlakMVP.GeneticAlgorithm.Genotype;

using indexT = System.Int32;

namespace SouvlakMVPTest
{
    [TestClass]
    public class TestGenotype
    {
        [TestMethod]
        public void OddVertices_getVertices()
        {
            indexT[] testUnevenVertices = { 0, 1, 2, 3 };
            Genotype testGenotype = new Genotype(testUnevenVertices);
            Assert.AreEqual(testGenotype.UnevenVerticesIdxs[0], testUnevenVertices[0]);
            Assert.AreEqual(testGenotype.UnevenVerticesIdxs[1], testUnevenVertices[1]);
            Assert.AreEqual(testGenotype[2], testUnevenVertices[2]);
            Assert.AreEqual(testGenotype[3], testUnevenVertices[3]);
        }

        [TestMethod]
        [ExpectedException(typeof(NonUniqueVerticesException),
            "Duplicate edges with different weights are not allowed!")]
        public void Genotype_not_unique_vertices()
        {
            indexT[] testUnevenVertices = { 0, 0, 0 };
            Genotype testGenotype = new Genotype(testUnevenVertices);
        }

        [TestMethod]
        [ExpectedException(typeof(NonEvenNumberOfVerticesException),
            "Genotype must have EVEN (ont odd) number of vertices!")]
        public void Genotype_odd_number_of_vertices()
        {
            indexT[] testUnevenVertices = { 0, 1, 2 };
            Genotype testGenotype = new Genotype(testUnevenVertices);
        }

        [TestMethod]
        public void GetPairs_get_vertex_pairs()
        {
            indexT[] testUnevenVertices = { 0, 1, 2, 3 };

            Genotype testGenotype = new Genotype(testUnevenVertices);

            (indexT start, indexT stop)[] testPairsList = { (0, 1), (2, 3) };
            (indexT start, indexT stop)[] testGenotypePairsList = testGenotype.GetPairs();
            Assert.AreEqual(testGenotypePairsList[0], testPairsList[0]);
            Assert.AreEqual(testGenotypePairsList[1], testPairsList[1]);
        }

        [TestMethod]
        public void Length_get_length()
        {
            indexT[] testUnevenVertices = { 0, 1, 2, 3 }; ;

            Genotype testGenotype = new Genotype(testUnevenVertices);
            Assert.AreEqual(testGenotype.Length, 4);
        }
    }
}