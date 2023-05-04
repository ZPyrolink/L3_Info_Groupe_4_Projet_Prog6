using System.Drawing;

namespace Taluva.Model;

public class Chunk
{
	public Cell[] Coords { get; private set; }
	private int rotation;
	public readonly int Level=1;

	public Chunk(int l, Cell left, Cell right)
	{
		this.Coords = new Cell[3];
		this.Coords[0] = new Cell(Biomes.Volcano);
		this.Coords[1] = left;
		this.Coords[2] = right;
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