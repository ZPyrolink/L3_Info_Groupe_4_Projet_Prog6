using System.Drawing;

namespace Taluva.Model;

public class Chunk
{
	private Point[] coords;
	private int rotation;

	public Chunk()
	{
		this.coords = new Point[3];
	}
}