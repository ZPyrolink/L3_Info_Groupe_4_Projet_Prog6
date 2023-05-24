using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using UnityEngine;

using Taluva.Model;
using Taluva.Utils;

namespace TestsTaluva
{
    [TestFixture]
    public class TestBoard
    {
        private Board _board;
        private DynamicMatrix<Cell> _matrix;
        private Player _player1;
        private Player _player2;

        [SetUp]
        public void init()
        {
            _board = new();
            _matrix = _board.WorldMap;
            _player1 = new(PlayerColor.Blue);
            _player2 = new(PlayerColor.Red);
            Chunk _chunk = new(1, new(Biomes.Desert), new(Biomes.Plain));
            PointRotation _pointRot = new(new(0, 0), Rotation.N);
            _board.AddChunk(_chunk, _player1, _pointRot, Rotation.N);
        }

        [Test]
        public void TestChunkSlotAfterInit()
        {
            _board = new();
            PointRotation[] possible = _board.GetChunkSlots();
            Assert.AreEqual(0, possible[0].Point.x);
            Assert.AreEqual(0, possible[0].Point.y);
            foreach (bool rot in possible[0].Rotations)
            {
                Assert.IsTrue(rot);
            }
        }

        [Test]
        public void TestAddChunk()
        {
            DynamicMatrix<Cell> _map = _board.WorldMap;

            Assert.IsFalse(_map.IsVoid(new(0, 0)));
            Assert.IsFalse(_map.IsVoid(new(-1, -1)));
            Assert.IsFalse(_map.IsVoid(new(-1, 0)));

            Assert.AreEqual(Biomes.Volcano, _map[new(0, 0)].CurrentBiome);
            Assert.AreEqual(Biomes.Plain, _map[new(-1, -1)].CurrentBiome);
            Assert.AreEqual(Biomes.Desert, _map[new(-1, 0)].CurrentBiome);
        }

        [Test]
        public void TestChunkSlotAfterOneChunk()
        {
            PointRotation[] possible = _board.GetChunkSlots();
            List<PointRotation> neighbors = new()
            {
                new(new(-3, -2), new[] { false, false, true, true, false, false }),
                new(new(-3, -1), new[] { false, false, true, true, true, false }),
                new(new(-3, 0), new[] { false, false, true, true, true, false }),
                new(new(-3, 1), new[] { false, false, false, true, true, false }),

                new(new(-2, -2), new[] { false, true, true, true, false, false }),
                new(new(-2, -1), new[] { true, true, false, false, true, true }),
                new(new(-2, 0), new[] { true, true, false, false, false, true }),
                new(new(-2, 1), new[] { true, true, true, false, false, true }),
                new(new(-2, 2), new[] { false, false, false, true, true, true }),

                new(new(-1, -3), new[] { false, true, true, false, false, false }),
                new(new(-1, -2), new[] { true, false, false, true, true, true }),
                new(new(-1, 1), new[] { true, true, true, true, false, false }),
                new(new(-1, 2), new[] { false, false, false, false, true, true }),

                new(new(0, -2), new[] { true, true, true, false, false, false }),
                new(new(0, -1), new[] { false, false, false, true, true, true }),
                new(new(0, 1), new[] { false, true, true, true, false, false }),
                new(new(0, 2), new[] { true, false, false, false, true, true }),

                new(new(1, -2), new[] { true, true, true, false, false, false }),
                new(new(1, -1), new[] { false, false, true, true, true, true }),
                new(new(1, 0), new[] { false, true, true, true, true, false }),
                new(new(1, 1), new[] { true, false, false, false, true, true }),

                new(new(2, -1), new[] { true, true, false, false, false, false }),
                new(new(2, 0), new[] { true, true, false, false, false, true }),
                new(new(2, 1), new[] { true, false, false, false, false, true })
            };
            foreach (PointRotation pr in possible)
            {
                Assert.IsTrue(neighbors.Exists(p =>
                    p.Point.x == pr.Point.x && p.Point.y == p.Point.y && p.RotationEquals(pr)));
            }

            Assert.AreEqual(neighbors.Count, possible.Length);
        }

        [Test]
        public void TestPlaceBuilding()
        {
            _board.PlaceBuilding(_matrix[new(-1, -1)], Building.Barrack, _player1);
            Assert.AreEqual(Building.Barrack, _matrix[new(-1, -1)].CurrentBuildings);

            Chunk _chunk = new(1, new(Biomes.Lake), new(Biomes.Forest));
            PointRotation _pointRot = new(new(-3, 0), Rotation.S);
            _board.AddChunk(_chunk, _player2, _pointRot, Rotation.S);
            _board.PlaceBuilding(_matrix[new(-2, -0)], Building.Barrack, _player1);
            _board.PlaceBuilding(_matrix[new(-2, 1)], Building.Barrack, _player1);

            _board.PlaceBuilding(_matrix[new(-1, 0)], Building.Temple, _player1);
            Assert.AreEqual(Building.Temple, _matrix[new(-1, 0)].CurrentBuildings);

            _chunk = new(3, new(Biomes.Desert), new(Biomes.Plain));
            _pointRot = new(new(0, 1), Rotation.NE);
            _board.AddChunk(_chunk, _player2, _pointRot, Rotation.NE);

            _board.PlaceBuilding(_matrix[new(-1, 1)], Building.Tower, _player1);
            Assert.AreEqual(Building.Tower, _matrix[new(-1, 1)].CurrentBuildings);
        }

        [Test]
        public void TestGetBuildingSlotsAfterInit()
        {
            _board = new();
            Assert.AreEqual(0, _board.GetBarrackSlots(_player1).Length);
            Assert.AreEqual(0, _board.GetTempleSlots(new(PlayerColor.Blue)).Length);
            Assert.AreEqual(0, _board.GetTowerSlots(new(PlayerColor.Blue)).Length);
        }

        [Test]
        public void TestGetBarrackSlots()
        {
            Vector2Int[] possibles = _board.GetBarrackSlots(_player1);
            Vector2Int[] barrackSlots = new Vector2Int[2];

            barrackSlots[0] = new(-1, -1);
            barrackSlots[1] = new(-1, 0);

            foreach (Vector2Int p in possibles)
            {
                Assert.IsTrue(barrackSlots.Contains(p));
            }
        }

        [Test]
        public void TestGetTempleSlots()
        {
            Chunk _chunk = new(1, new(Biomes.Desert), new(Biomes.Plain));
            PointRotation _pointRot = new(new(0, 1), Rotation.NE);
            _board.AddChunk(_chunk, _player1, _pointRot, Rotation.NE);


            _board.PlaceBuilding(_matrix[new(-1, -1)], Building.Barrack, _player1);
            _board.PlaceBuilding(_matrix[new(-1, 0)], Building.Barrack, _player1);
            _board.PlaceBuilding(_matrix[new(-1, 1)], Building.Barrack, _player1);

            Vector2Int[] possible = _board.GetTempleSlots(_player1);
            Assert.AreEqual(1, possible.Length);
            Assert.AreEqual(0, possible[0].x);
            Assert.AreEqual(2, possible[0].y);

            _board.PlaceBuilding(_matrix[new(0, 2)], Building.Temple, _player1);
            _chunk = new(1, new(Biomes.Desert), new(Biomes.Plain));
            _pointRot = new(new(0, -1), Rotation.NW);
            _board.AddChunk(_chunk, _player2, _pointRot, Rotation.NW);

            possible = _board.GetTempleSlots(_player1);
            Assert.AreEqual(0, possible.Length);
        }

        [Test]
        public void TestGetTowerSlots()
        {
            Chunk _chunk = new(3, new(Biomes.Desert), new(Biomes.Plain));
            PointRotation _pointRot = new(new(0, 1), Rotation.NE);
            _board.AddChunk(_chunk, _player2, _pointRot, Rotation.NE);

            DynamicMatrix<Cell> _matrix = _board.WorldMap;
            _board.PlaceBuilding(_matrix[new(-1, 0)], Building.Barrack, _player1);

            Vector2Int[] possible = _board.GetTowerSlots(_player1);
            Assert.AreEqual(1, possible.Length);
            Assert.AreEqual(-1, possible[0].x);
            Assert.AreEqual(1, possible[0].y);

            _board.PlaceBuilding(_matrix[possible[0]], Building.Tower, _player1);
            _chunk = new(3, new(Biomes.Desert), new(Biomes.Plain));
            _pointRot = new(new(-3, 0), Rotation.S);
            _board.AddChunk(_chunk, _player2, _pointRot, Rotation.S);
            _chunk = new(3, new(Biomes.Desert), new(Biomes.Plain));
            _pointRot = new(new(-2, -2), Rotation.SE);
            _board.AddChunk(_chunk, _player2, _pointRot, Rotation.SE);

            possible = _board.GetTowerSlots(_player1);
            Assert.AreEqual(0, possible.Length);

            _board.PlaceBuilding(_matrix[new(-2, -1)], Building.Barrack, _player1);
            _board.PlaceBuilding(_matrix[new(-1, -2)], Building.Barrack, _player1);
            possible = _board.GetTowerSlots(_player1);
            Assert.AreEqual(0, possible.Length);
        }

        [Test]
        public void TestGetCellCoord()
        {
            Cell c1 = new(Biomes.Forest);
            Cell c2 = new(Biomes.Mountain);

            Chunk _chunk = new(1, c1, c2);
            PointRotation _pointRot = new(new(0, 1), Rotation.NE);
            _board.AddChunk(_chunk, _player1, _pointRot, Rotation.NE);

            Assert.AreEqual(0, _board.GetCellCoord(c1).x);
            Assert.AreEqual(2, _board.GetCellCoord(c1).y);

            Assert.AreEqual(-1, _board.GetCellCoord(c2).x);
            Assert.AreEqual(1, _board.GetCellCoord(c2).y);
        }
    }
}