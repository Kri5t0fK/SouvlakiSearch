using Microsoft.VisualStudio.TestTools.UnitTesting;
using SouvlakMVP;

namespace SouvlakMVPTest
{
    [TestClass]
    public class TestMap
    {
        [TestMethod]
        public void ToStringTest()
        {
            //Creating graph
            Map graph = new Map();
            string expected = string.Empty;

            Assert.AreEqual(expected, graph.ToString());

        }
    }
}