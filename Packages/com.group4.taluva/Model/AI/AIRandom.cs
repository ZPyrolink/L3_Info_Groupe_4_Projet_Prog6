using Taluva.Controller;
using UnityEngine;

namespace Taluva.Model.AI
{
    public class AIRandom : AI
    {
        public AIRandom(PlayerColor id,GameManagment gm) : base(id,gm,null)
        {
            
        }
        public AIRandom(AIRandom original) : base(original)
        {
            
        }

        public override Player Clone()
        {
            return new AIRandom(this);
        }

        public override PointRotation PlayChunk()
        {
            int rand;
            int max = 0;
            PointRotation[] possible = this.gm.gameBoard.GetChunkSlots();
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

        public override (Building buil, Vector2Int pos) PlayBuild()
        {
            int rand = Random.Range(0,1000);
            Vector2Int[] temples = gm.gameBoard.GetTempleSlots(this);
            if(temples.Length>0)
            {
                rand = Random.Range(0, temples.Length);
                return (Building.Barrack,temples[rand]);
            } 
            Vector2Int[] towers = gm.gameBoard.GetTowerSlots(this);
            if(towers.Length>0)
            {
                rand = Random.Range(0, towers.Length);
                return (Building.Tower,towers[rand]);
            }
            Vector2Int[] barracks = gm.gameBoard.GetBarrackSlots(this);
            rand = Random.Range(0, barracks.Length);
            return (Building.Barrack,barracks[rand]);
        }

        
    }
}