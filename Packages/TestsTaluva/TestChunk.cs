using NUnit.Framework;
using Taluva.Model;

namespace TestsTaluva
{
    public class ChunkTests
    {
        [Test]
        public void Constructor_CopyConstructor()
        {
            // Arrange
            var originalCell1 = new Cell(Biomes.Desert);
            var originalCell2 = new Cell(Biomes.Forest);
            var originalChunk = new Chunk(1, originalCell1, originalCell2);

            // Act
            var copiedChunk = new Chunk(originalChunk);

            // Assert
            Assert.IsNotNull(copiedChunk);
            Assert.IsFalse(ReferenceEquals(originalChunk , copiedChunk));
            Assert.AreEqual(originalChunk.Level, copiedChunk.Level);
            Assert.AreEqual(originalChunk.Coords[0].CurrentBiome, copiedChunk.Coords[0].CurrentBiome);
            Assert.AreEqual(originalChunk.Coords[1].CurrentBiome, copiedChunk.Coords[1].CurrentBiome);
            Assert.AreEqual(originalChunk.Coords[2].CurrentBiome, copiedChunk.Coords[2].CurrentBiome);
            Assert.AreEqual(originalChunk.Coords[1].Owner, copiedChunk.Coords[1].Owner);
            Assert.AreEqual(originalChunk.Coords[2].Owner, copiedChunk.Coords[2].Owner);
            Assert.AreEqual(originalChunk.Coords[1].CurrentBuildings, copiedChunk.Coords[1].CurrentBuildings);
            Assert.AreEqual(originalChunk.Coords[2].CurrentBuildings, copiedChunk.Coords[2].CurrentBuildings);
        }
    }
}