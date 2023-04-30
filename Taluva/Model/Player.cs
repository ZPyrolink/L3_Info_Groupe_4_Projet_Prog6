public class Player
{
	private Chunk lastChunk;
	private bool bPlayed;
    private PlayerColor ID;
	private int nbTowers = 2;
	private int nbTemple = 3;
	private int nbBarrack = 20;

	public Player(PlayerColor ID)
	{
		this.ID = ID;
	}
}
