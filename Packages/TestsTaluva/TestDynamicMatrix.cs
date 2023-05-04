// using Taluva.Utils;
// using System.Drawing;
//
// namespace TestDynamicMatrix
// {
//     [TestClass]
//     public class TestDynamicMatrix
//     {
//         private DynamicMatrix<String> matrix;
//
//         [TestInitialize]
//         public void Init()
//         {
//             matrix = new DynamicMatrix<String>();
//             matrix.Add("A", new Point(0, 0));
//             matrix.Add("B", new Point(0, 1));
//             matrix.Add("C", new Point(1, 0));
//         }
//
//         [TestMethod]
//         public void TestAjout()
//         {
//             Assert.AreEqual("A", matrix.GetValue(new Point(0, 0)));
//             Assert.AreEqual("B", matrix.GetValue(new Point(0, 1)));
//             Assert.AreEqual("C", matrix.GetValue(new Point(1, 0)));
//         }
//
//         [TestMethod]
//         public void TestVoid()
//         {
//             Assert.IsFalse(matrix.IsVoid(new Point(0, 0)));
//             Assert.IsFalse(matrix.IsVoid(new Point(0, 1)));
//             Assert.IsFalse(matrix.IsVoid(new Point(1, 0)));
//
//             Assert.IsTrue(matrix.IsVoid(new Point(2, 0)));
//             Assert.IsTrue(matrix.IsVoid(new Point(0, 2)));
//             Assert.IsTrue(matrix.IsVoid(new Point(2, 2)));
//         }
//
//         [TestMethod]
//         public void TestReplace()
//         {
//             matrix.Add("L", new Point(0, 0));
//             matrix.Add("K", new Point(1, 0));
//
//             Assert.AreNotEqual("A", matrix.GetValue(new Point(0, 0)));
//             Assert.AreNotEqual("C", matrix.GetValue(new Point(1, 0)));
//
//             Assert.AreEqual("L", matrix.GetValue(new Point(0, 0)));
//             Assert.AreEqual("K", matrix.GetValue(new Point(1, 0)));
//         }
//     }
// }