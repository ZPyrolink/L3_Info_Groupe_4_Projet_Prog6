using System.Drawing;
using System.Drawing;

namespace Taluva.Model;

public class Chunk
{
	public Point[] Coords { get; private set; }
	private int rotation;

	public Chunk()
	{
		this.Coords = new Point[3];
	}
}