using System;

public class Player
{
	Chunk lastChunk;
	bool b_played;
    PlayerColor ID;
	int nbTowers = 2;
	int nbTemple = 3;
	int nbBarrack = 20;

	public Player(PlayerColor ID)
	{
		this.ID = ID;
	}
}
