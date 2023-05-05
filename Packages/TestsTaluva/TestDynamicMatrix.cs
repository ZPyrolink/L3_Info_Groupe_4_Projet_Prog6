using NUnit.Framework;

using Taluva.Utils;

namespace TestsTaluva
{
    [TestFixture]
    public class TestDynamicMatrix
    {
        private DynamicMatrix<string> _matrix;

        [SetUp]
        public void Init()
        {
            _matrix = new()
            {
                { "A", new(0, 0) },
                { "B", new(0, 1) },
                { "C", new(1, 0) }
            };
        }

        [Test]
        public void TestAjout()
        {
            Assert.AreEqual("A", _matrix.GetValue(new(0, 0)));
            Assert.AreEqual("B", _matrix.GetValue(new(0, 1)));
            Assert.AreEqual("C", _matrix.GetValue(new(1, 0)));
        }

        [Test]
        public void TestVoid()
        {
            Assert.IsFalse(_matrix.IsVoid(new(0, 0)));
            Assert.IsFalse(_matrix.IsVoid(new(0, 1)));
            Assert.IsFalse(_matrix.IsVoid(new(1, 0)));

            Assert.IsTrue(_matrix.IsVoid(new(2, 0)));
            Assert.IsTrue(_matrix.IsVoid(new(0, 2)));
            Assert.IsTrue(_matrix.IsVoid(new(2, 2)));
        }

        [Test]
        public void TestReplace()
        {
            _matrix.Add("L", new(0, 0));
            _matrix.Add("K", new(1, 0));

            Assert.AreNotEqual("A", _matrix.GetValue(new(0, 0)));
            Assert.AreNotEqual("C", _matrix.GetValue(new(1, 0)));

            Assert.AreEqual("L", _matrix.GetValue(new(0, 0)));
            Assert.AreEqual("K", _matrix.GetValue(new(1, 0)));
        }
    }
}