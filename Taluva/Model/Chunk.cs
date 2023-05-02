using System.Drawing;

namespace Taluva.Model;

public class Chunk
{
	public Cell[] Coords { get; private set; }
	private int rotation;
	private int level;

	public Chunk()
	{
		this.Coords = new Cell[3];
		
	}
	public bool CanRotate()
	{
		throw new NotImplementedException();
	}

	public void RotateChunk()
	{
		throw new NotImplementedException();
	}
}