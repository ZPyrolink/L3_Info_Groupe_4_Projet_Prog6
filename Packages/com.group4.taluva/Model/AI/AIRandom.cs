using System.Linq;

using Taluva.Controller;
using UnityEngine;

namespace Taluva.Model.AI
{
    public class AIRandom : AI
    {
        public AIRandom(Color id,GameManagment gm) : base(id,gm)
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
            PointRotation[] possible = this.Gm.gameBoard.GetChunkSlots();
            int max = possible.SelectMany(p => p.Rotations).Count(rot => rot);

            rand = Random.Range(1, max + 1);
            foreach (PointRotation p in possible)
                for (int i = 0; i < 6; i++)
                {
                    if (p.Rotations[i])
                        rand--;

                    if (rand == 0)
                        return new(p.Point, (Rotation) i);
                }

            return null;
        }

        public override (Building buil, Vector2Int pos) PlayBuild()
        {
            int rand;
            Vector2Int[] temples = Gm.gameBoard.GetTempleSlots(this);
            if(temples.Length>0)
            {
                rand = Random.Range(0, temples.Length);
                return (Building.Temple,temples[rand]);
            } 
            Vector2Int[] towers = Gm.gameBoard.GetTowerSlots(this);
            if(towers.Length>0)
            {
                rand = Random.Range(0, towers.Length);
                return (Building.Tower,towers[rand]);
            }
            Vector2Int[] barracks = Gm.gameBoard.GetBarrackSlots(this);
            rand = Random.Range(0, barracks.Length);
            return (Building.Barrack,barracks[rand]);
        }
    }
}