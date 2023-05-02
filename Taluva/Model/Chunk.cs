using System.Drawing;

namespace Taluva.Model;

public class Chunk
{
	public Cell[] Coords { get; private set; }
	private int rotation;
	private int Level=1;

	public Chunk(int l)
	{
		this.Coords = new Cell[3];
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