using UnityEngine;

namespace Taluva.Model.AI
{
    public class AIRandom : AI
    {
        public AIRandom(PlayerColor id,Board board) : base(id)
        {
            
        }

        public PointRotation PlayChunk()
        {
            int rand;
            int max = 0;
            PointRotation[] possible = this.board.GetChunkSlots();
            foreach (PointRotation p in possible)
            {
                foreach (bool rot in p.rotations)
                {
                    if (rot)
                    {
                        max++;
                    }
                }
            }

            rand = Random.Range(0, max);
            foreach (PointRotation p in possible)
            {
                for (int i = 0; i<6;i++)
                {
                    if (p.rotations[i])
                    {
                        rand--;
                    }

                    if (rand == 0)
                    {
                        return new PointRotation(p.point, (Rotation)i);
                    }
                }
            }
            
            return null;}

        public (Building buil, Vector2Int? pos) PlayBuild()
        {
            int rand = Random.Range(0,1000);
            Vector2Int[] temples = board.GetTempleSlots(this);
            if(temples.Length>0)
            {
                rand = Random.Range(0, temples.Length);
                return (Building.Barrack,temples[rand]);
            } 
            Vector2Int[] towers = board.GetTowerSlots(this);
            if(towers.Length>0)
            {
                rand = Random.Range(0, towers.Length);
                return (Building.Tower,towers[rand]);
            }
            Vector2Int[] barracks = board.GetBarrackSlots();
            rand = Random.Range(0, barracks.Length);
            return (Building.Barrack,barracks[rand]);
        }

        
    }
}