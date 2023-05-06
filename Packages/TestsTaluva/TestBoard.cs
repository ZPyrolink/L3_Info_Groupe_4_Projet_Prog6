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
        private Player _player1;
        private Player _player2;

        [SetUp]
        public void init()
        {
            _board = new();
            _player1 = new(PlayerColor.Blue);
            _player2 = new(PlayerColor.Red);
            Chunk _chunk = new(1, new(Biomes.Desert), new(Biomes.Plain));
            PointRotation _pointRot = new(new(0,0),Rotation.N);
            _board.AddChunk(_chunk,_player1,_pointRot);
        }
        
        [Test]
        public void TestChunkSlotAfterInit()
        {
            _board = new();
            PointRotation[] possible= _board.GetChunkSlots();
            Assert.AreEqual(0,possible[0].point.x);
            Assert.AreEqual(0,possible[0].point.y);
            foreach (bool rot in possible[0].rotations)
            {
                Assert.IsTrue(rot);
            }
        }

        [Test]
        public void TestAddChunk()
        {
            DynamicMatrix<Cell> _map = _board.GetMatrix();
            
            Assert.IsFalse(_map.IsVoid(new(0,0)));
            Assert.IsFalse(_map.IsVoid(new(-1,-1)));
            Assert.IsFalse(_map.IsVoid(new(-1,0)));
            
            Assert.AreEqual(Biomes.Volcano, _map.GetValue(new(0,0)).ActualBiome);
            Assert.AreEqual(Biomes.Plain, _map.GetValue(new(-1,-1)).ActualBiome);
            Assert.AreEqual(Biomes.Desert, _map.GetValue(new(-1,0)).ActualBiome);
        }
        
        [Test]
        public void TestChunkSlotAfterOneChunk()
        {
            PointRotation[] possible= _board.GetChunkSlots();
            List<PointRotation> neighbors = new()
            {
                new(new(-3, -2),new[]{false,false,true,true,false,false}),
                new(new(-3, -1),new[]{false,false,true,true,true,false}),
                new(new(-3, 0), new[]{false,false,true,true,true,false}),
                new(new(-3, 1), new[]{false,false,false,true,true,false}),

                new(new(-2, -2),new[]{false,true,true,true,false,false}),
                new(new(-2, -1),new[]{true,true,false,false,true,true}),
                new(new(-2, 0), new[]{true,true,false,false,false,true}),
                new(new(-2, 1), new[]{true,true,true,false,false,true}),
                new(new(-2, 2), new[]{false,false,false,true,true,true}),

                new(new(-1, -3),new[]{false,true,true,false,false,false}),
                new(new(-1, -2),new[]{true,false,false,true,true,true}),
                new(new(-1, 1), new[]{true,true,true,true,false,false}),
                new(new(-1, 2), new[]{false,false,false,false,true,true}),

                new(new(0, -2),new[]{true,true,true,false,false,false}),
                new(new(0, -1),new[]{false,false,false,true,true,true}),
                new(new(0, 1), new[]{false,true,true,true,false,false}),
                new(new(0, 2), new[]{true,false,false,false,true,true}),

                new(new(1, -2),new[]{true,true,true,false,false,false}),
                new(new(1, -1),new[]{false,false,true,true,true,true}),
                new(new(1, 0), new[]{false,true,true,true,true,false}),
                new(new(1, 1), new[]{true,false,false,false,true,true}),

                new(new(2, -1),new[]{true,true,false,false,false,false}),
                new(new(2, 0), new[]{true,true,false,false,false,true}),
                new(new(2, 1), new[]{true,false,false,false,false,true})
            };
            foreach (PointRotation pr in possible)
            {
                Assert.IsTrue(neighbors.Exists(p => p.point.x == pr.point.x && p.point.y == p.point.y && p.RotationEquals(pr)));
            }
        }
        
    }
}