﻿using NUnit.Framework;
using System.Globalization;
using System;
using Taluva.Controller;
using Taluva.Model;
using UnityEngine;
using Taluva.Model.AI;
using System.Linq;

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
            Assert.AreEqual(47, _gm.Pile.Content.Count);
            Assert.AreEqual(1, _gm.Pile.Played.Count);
            _gm.ValidateTile(new(new(0, 0), new[] { true, false, false, false, false, false }), Rotation.N);
            _gm.ValidateBuilding(_gm.gameBoard.WorldMap[new(-1, 0)], Building.Barrack);

            _gm.Undo();
            Assert.IsTrue(_gm.gameBoard.WorldMap[new(-1, 0)].CurrentBuildings == Building.None);
            Assert.AreEqual(20, _gm.CurrentPlayer.NbBarrack);

            _gm.CurrentChunk = _gm.Pile.Draw();
            _gm.ValidateTile(new(new(0, 1), new[] { false, false, false, true, false, false }), Rotation.S);
            _gm.ValidateBuilding(_gm.gameBoard.WorldMap[new(-1, 0)], Building.Barrack);
            _gm.ValidateBuilding(_gm.gameBoard.WorldMap[new(-1, -1)], Building.Barrack);
            _gm.CurrentChunk = _gm.Pile.Draw();
            _gm.ValidateTile(new(new(0, 1), new[] { false, false, false, false, false, true }), Rotation.NW);

            _gm.Undo();
            Assert.IsTrue(_gm.gameBoard.WorldMap[new(-1, 0)].CurrentBuildings == Building.Barrack);

            
        }

        [Test]
        public void TestRedo()
        {
            _gm.ValidateTile(new(new(0, 0), new[] { true, false, false, false, false, false }), Rotation.N);
            _gm.Undo();
            Assert.IsTrue(_gm.IsVoid(new(0, 0)));
            _gm.Redo();
            Assert.IsFalse(_gm.IsVoid(new(0, 0)));
            Assert.IsFalse(_gm.IsVoid(new(-1, 0)));
            Assert.IsFalse(_gm.IsVoid(new(-1, -1)));
            Assert.AreEqual(47, _gm.Pile.Content.Count);
            Assert.AreEqual(1, _gm.Pile.Played.Count);

            _gm.ValidateBuilding(_gm.gameBoard.WorldMap[new(-1, 0)], Building.Barrack);

            _gm.Undo();
            _gm.Redo();
            _gm.NextPlayer();

            Assert.IsTrue(_gm.gameBoard.WorldMap[new(-1, 0)].CurrentBuildings == Building.Barrack);
            Assert.AreEqual(19, _gm.CurrentPlayer.NbBarrack);

            _gm.CurrentChunk = _gm.Pile.Draw();
            _gm.ValidateTile(new(new(0, 1), new[] { false, false, false, true, false, false }), Rotation.S);
            _gm.ValidateBuilding(_gm.gameBoard.WorldMap[new(-1, -1)], Building.Barrack);
            _gm.CurrentChunk = _gm.Pile.Draw();
            _gm.ValidateTile(new(new(0, 1), new[] { false, false, false, false, false, true }), Rotation.NW);

            _gm.Undo();  
            _gm.Redo();
            Assert.AreEqual(46, _gm.Pile.Content.Count);
            Assert.AreEqual(18, _gm.CurrentPlayer.NbBarrack);
        }

        [Test]
        public void TestSave1Tile()
        {
            _gm.ValidateTile(new(new(0, 0), new[] { true, false, false, false, false, false }), Rotation.N);
            string path = "save1";

            _gm.Save(path);

            GameManagment gm = new(2, Enumerable.Repeat(typeof(AIRandom), 0).ToArray());
            gm.LoadGame(path);
            Assert.IsFalse(gm.IsVoid(new(0, 0)));
            Assert.IsFalse(gm.IsVoid(new(-1, 0)));
            Assert.IsFalse(gm.IsVoid(new(-1, -1)));
            Assert.AreEqual(_gm.Pile.Content.Count, gm.Pile.Content.Count);

            Assert.AreEqual(_gm.CurrentChunk.Coords[1].CurrentBiome, gm.CurrentChunk.Coords[1].CurrentBiome);
            Assert.AreEqual(_gm.CurrentChunk.Coords[2].CurrentBiome, gm.CurrentChunk.Coords[2].CurrentBiome);
            Assert.AreEqual(_gm.gameBoard.WorldMap[new(0, 0)].CurrentBiome, gm.gameBoard.WorldMap[new(0, 0)].CurrentBiome);
            Assert.AreEqual(_gm.gameBoard.WorldMap[new(-1, 0)].CurrentBiome, gm.gameBoard.WorldMap[new(-1, 0)].CurrentBiome);
            Assert.AreEqual(_gm.gameBoard.WorldMap[new(-1, -1)].CurrentBiome, gm.gameBoard.WorldMap[new(-1, -1)].CurrentBiome);

            Assert.AreEqual(_gm.gameBoard.WorldMap[new(0, 0)].CurrentBuildings, gm.gameBoard.WorldMap[new(0, 0)].CurrentBuildings);
            Assert.AreEqual(_gm.gameBoard.WorldMap[new(-1, 0)].CurrentBuildings, gm.gameBoard.WorldMap[new(-1, 0)].CurrentBuildings);
            Assert.AreEqual(_gm.gameBoard.WorldMap[new(-1, -1)].CurrentBuildings, gm.gameBoard.WorldMap[new(-1, -1)].CurrentBuildings);

        }

        [Test]
        public void TestSave1Building()
        {
            _gm.ValidateTile(new(new(0, 0), new[] { true, false, false, false, false, false }), Rotation.N);
            _gm.Phase2(new(-1, 0), Building.Barrack);

            string path = "save1";
            _gm.Save(path);
            GameManagment gm = new(2);
            gm.LoadGame(path);

            Assert.AreEqual(Building.Barrack, gm.gameBoard.WorldMap[new(-1, 0)].CurrentBuildings);
            Assert.AreEqual(19, gm.PreviousPlayer.NbBarrack);
        }


    }
}
