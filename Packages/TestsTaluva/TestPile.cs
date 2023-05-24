using System;
using NUnit.Framework;
using System.Linq;
using Taluva.Model;

namespace TestsTaluva
{
    public class TestPile
    {
        public class CloneableInteger : ICloneable
        {
            public int Value { get; set; }

            public CloneableInteger(int value)
            {
                Value = value;
            }

            public object Clone()
            {
                return new CloneableInteger(this.Value);
            }

            public override bool Equals(object obj)
            {
                if (obj is CloneableInteger other)
                {
                    return this.Value == other.Value;
                }
                return false;
            }

            public override int GetHashCode()
            {
                return Value;
            }
        }

        [Test]
        public void TestPileClone()
        {
            // Arrange
            var originalPile = new Pile<CloneableInteger>(new CloneableInteger[] { new CloneableInteger(1), new CloneableInteger(2), new CloneableInteger(3), new CloneableInteger(4), new CloneableInteger(5) });
            originalPile.Draw();

            // Act
            var clonePile = new Pile<CloneableInteger>(originalPile);
            Assert.AreEqual(originalPile.Content.Count, clonePile.Content.Count);
            Assert.AreEqual(originalPile.Played.Count, clonePile.Played.Count);
            Assert.IsTrue(originalPile.Content.SequenceEqual(clonePile.Content));
            Assert.IsFalse(ReferenceEquals(originalPile.Content, clonePile.Content));
            Assert.IsTrue(originalPile.Played.SequenceEqual(clonePile.Played));
            Assert.IsFalse(ReferenceEquals(originalPile.Played, clonePile.Played));
            for (int i = 0; i < originalPile.Content.Count; i++)
            {
                CloneableInteger originalC = originalPile.Content.Pop();
                CloneableInteger cloneC = clonePile.Content.Pop();
                Assert.IsFalse(ReferenceEquals(originalC,cloneC ));
                Assert.AreEqual(originalC.Value,cloneC.Value);
            }
        }
    }
}