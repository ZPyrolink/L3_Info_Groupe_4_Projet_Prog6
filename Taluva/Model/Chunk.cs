using System.Drawing;

namespace Taluva.Model;

public class Chunk
{
	public Cell[] Coords { get; private set; }
	private int rotation;

	public Chunk()
	{
		this.Coords = new Cell[3];
	}
}