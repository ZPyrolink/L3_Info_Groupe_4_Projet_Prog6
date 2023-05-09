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
            _gm.InitPlay();
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

            
            _gm.ValidateTile(new(new(0, 0), new[] { true, false, false, false, false, false }), Rotation.N);
            Assert.IsTrue(_gm.CanUndo);

            _gm.Undo();
            Assert.IsFalse(_gm.CanUndo);
        }

        [Test]
        public void TestCanRedo()
        {
            Assert.IsFalse( _gm.CanRedo);
            _gm.ValidateTile(new(new(0, 0), new[] { true, false, false, false, false, false }), Rotation.N);

            Assert.IsFalse(_gm.CanRedo);
            _gm.Undo();

            Assert.IsTrue(_gm.CanRedo);
            _gm.Redo();

            Assert.IsFalse(_gm.CanRedo);
        }

        [Test]
        public void TestUndo()
        {
            _gm.ValidateTile(new(new(0, 0), new[] { true, false, false, false, false, false }), Rotation.N);
            _gm.Undo();
            Assert.IsTrue(_gm.IsVoid(new(0, 0)));
            Assert.IsTrue(_gm.IsVoid(new(-1, 0)));
            Assert.IsTrue(_gm.IsVoid(new(-1, -1)));

            _gm.ValidateTile(new(new(0, 0), new[] { true, false, false, false, false, false }), Rotation.N);
            _gm.PlaceBuilding(_gm.gameBoard.WorldMap.GetValue(new(-1, 0)), Building.Barrack);

            _gm.Undo();
            Assert.IsTrue(_gm.gameBoard.WorldMap.GetValue(new(-1, 0)).ActualBuildings == Building.None);
        }

        [Test]
        public void TestRedo()
        {
            _gm.ValidateTile(new(new(0, 0), new[] { true, false, false, false, false, false }), Rotation.N);
            _gm.Undo();
            _gm.Redo();
            Assert.IsFalse(_gm.IsVoid(new(0, 0)));
            Assert.IsFalse(_gm.IsVoid(new(-1, 0)));
            Assert.IsFalse(_gm.IsVoid(new(-1, -1)));

            _gm.PlaceBuilding(_gm.gameBoard.WorldMap.GetValue(new(-1, 0)), Building.Barrack);

            _gm.Undo();
            _gm.Redo();
            Assert.IsTrue(_gm.gameBoard.WorldMap.GetValue(new(-1, 0)).ActualBuildings == Building.Barrack);
        }
    }
}
