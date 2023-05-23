using System;
using NUnit.Framework;
using Taluva.Controller;
using Taluva.Model;
using Taluva.Model.AI;
using UnityEngine;

namespace TestsTaluva
{
    public class TestGameManagment
    {
        [Test]
        public void TestGameManagmentClone()
        {
            // Arrange
            Type[] typeAI = { typeof(AIRandom), typeof(AITree) };
            GameManagment original = new GameManagment(3, typeAI);
            // Act
            GameManagment copy = new GameManagment(original);

            // Assert
            // Verify that both instances have the same state
            Assert.AreEqual(original.NbPlayers, copy.NbPlayers);
            Assert.AreEqual(original.ActualPlayerIndex, copy.ActualPlayerIndex);
            Assert.AreEqual(original.MaxTurn, copy.MaxTurn);

            // Verify that the game board has been copied correctly
            Assert.AreEqual(original.gameBoard.WorldMap, copy.gameBoard.WorldMap);

            // Verify that they are not the same object
            Assert.IsFalse(ReferenceEquals(original, copy));
        }
        
    }
}