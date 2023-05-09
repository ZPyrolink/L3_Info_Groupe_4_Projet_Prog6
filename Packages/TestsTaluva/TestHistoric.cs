using NUnit.Framework;
using UnityEngine;

using Taluva.Controller;
using Taluva.Model;
using Taluva.Utils;

namespace TestsTaluva
{
    [TestFixture]
    public class TestHistoric
    {
        private GameManagment _gm;

        [SetUp]
        public void Init()
        {
            _gm = new(2);
        }

        [Test]
        public void TestAfterInit()
        {
            Assert.IsFalse(_gm.CanUndo);
            Assert.IsFalse(_gm.CanRedo);
        }

        [Test]
        public void TestCanUndo()
        {
            Assert.IsFalse(_gm.CanUndo);

            Chunk _chunk = new(1, new(Biomes.Desert), new(Biomes.Plain));
            _gm.AddHistoric(new(0,0), Rotation.N, _chunk);
            Assert.IsTrue(_gm.CanUndo);



        }

    }
}
