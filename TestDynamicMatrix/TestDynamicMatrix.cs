using Taluva.Utils;
using System.Drawing;

namespace TestDynamicMatrix
{
    [TestClass]
    public class TestDynamicMatrix
    {
        private DynamicMatrix<String> matrix;

        [TestInitialize] 
        public void Init() { 
            matrix = new DynamicMatrix<String>();
        }

        [TestMethod]
        public void TestAjout()
        {
            matrix.Add("A", new Point(0, 0));
            matrix.Add("B", new Point(0, 1));
            matrix.Add("C", new Point(1, 0));

            Assert.AreEqual("A", matrix.GetValue(new Point(0, 0)));
            Assert.AreEqual("B", matrix.GetValue(new Point(0, 1)));
            Assert.AreEqual("C", matrix.GetValue(new Point(1, 0)));
        }
    }
}