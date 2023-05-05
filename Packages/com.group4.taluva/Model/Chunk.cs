﻿using System;

namespace Taluva.Model
{
	public class Chunk
	{
		public Cell[] Coords { get; private set; }
		public Rotation rotation;
		public readonly int Level = 1;

		public Chunk(int l, Cell left, Cell right)
		{
			this.Coords = new Cell[3];
			this.Coords[0] = new(Biomes.Volcano, this);
			this.Coords[1] = left;
			left.parentCunk = this;
			this.Coords[2] = right;
			right.parentCunk = this;
			this.Level = l;

		}

		void RotateChunk()
		{
			throw new NotImplementedException();
		}

		private bool CanRotate()
		{
			throw new NotImplementedException();
		}
	}
}