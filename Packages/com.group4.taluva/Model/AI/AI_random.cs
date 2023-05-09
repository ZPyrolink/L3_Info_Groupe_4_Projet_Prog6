using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

namespace Taluva.Model
{
    public class AIRandom : AI
    {
        public AIRandom(PlayerColor id) : base(id)
        {
            
        }
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
            return (Building.None, null);
        }

        
    }
}