﻿namespace Taluva.Model
{
    public class Cell
    {
        /// <summary>
        /// Gets the actual biome of the cell.
        /// </summary>
        public readonly Biomes ActualBiome;

        /// <summary>
        /// Gets or sets the actual buildings on the cell.
        /// </summary>
        public Building ActualBuildings { get; set; }

        /// <summary>
        /// Gets or sets the owner of the cell.
        /// </summary>
        public PlayerColor Owner { get; set; }

        /// <summary>
        /// Gets or sets the parent chunk of the cell.
        /// </summary>
        public Chunk ParentChunk;

        /// <summary>
        /// Initializes a new instance of the Cell class with the specified biome and parent chunk.
        /// </summary>
        /// <param name="biome">The actual biome of the cell.</param>
        /// <param name="c">The parent chunk of the cell.</param>
        public Cell(Biomes biome, Chunk c)
        {
            this.ActualBiome = biome;
            this.ActualBuildings = Building.None;
            this.ParentChunk = c;
        }

        /// <summary>
        /// Initializes a new instance of the Cell class from an existing cell.
        /// </summary>
        /// <param name="c">The existing cell to copy.</param>
        public Cell(Cell c)
        {
            this.ActualBiome = c.ActualBiome;
            this.ActualBuildings = c.ActualBuildings;
            this.Owner = c.Owner;
            this.ParentChunk = c.ParentChunk;
        }

        /// <summary>
        /// Initializes a new instance of the Cell class with the specified biome.
        /// </summary>
        /// <param name="biome">The actual biome of the cell.</param>
        public Cell(Biomes biome)
        {
            this.ActualBiome = biome;
            this.ActualBuildings = Building.None;
        }
        
        /// <summary>
        /// Gets a value indicating whether the cell is buildable.
        /// </summary>
        public bool IsBuildable => ActualBiome != Biomes.None && ActualBiome != Biomes.Volcano && ActualBuildings == Building.None;

        /// <summary>
        /// Places a building on the cell.
        /// </summary>
        /// <param name="building">The building to place on the cell.</param>
        public void Build(Building building)
        {
            this.ActualBuildings = building;
        }

        /// <summary>
        /// Returns a string representation of the cell.
        /// </summary>
        /// <returns>The string representation of the cell.</returns>
        public override string ToString() => ActualBiome switch
        {
            Biomes.Desert => "D",
            Biomes.Forest => "F",
            Biomes.Lake => "L",
            Biomes.Mountain => "M",
            Biomes.Plain => "P",
            Biomes.Volcano => "V",
            _ => ""
        };

        /// <summary>
        /// Checks if the cell contains any building.
        /// </summary>
        /// <returns>True if the cell contains a building, false otherwise.</returns>
        public bool ContainsBuilding() => ActualBuildings != Building.None;
    }
}
