using System.Collections.Generic;
using System.Drawing;
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
            PointRotation[] possible = Gm.gameBoard.GetChunkSlots();
            
            List<PointRotation> possibleOcean = new List<PointRotation>();
            List<PointRotation> possibleVolcano =  new List<PointRotation>();
            foreach (var pr in possible)
            {
                if (Gm.gameBoard.WorldMap.IsVoid(pr.Point))
                    possibleOcean.Add(pr);
                else
                    possibleVolcano.Add(pr);
            }

            int max = 0;
            List<PointRotation> possiblePointRotations;
            if (possibleVolcano.Count > 0)
            {
                possiblePointRotations = possibleVolcano;
            }
            else
            {
                possiblePointRotations = possibleOcean;
            }
            max = possiblePointRotations.SelectMany(p => p.Rotations).Count(rot => rot);


            rand = Random.Range(1, max + 1);
            foreach (PointRotation p in possiblePointRotations)
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
            int maxlevel = 0;
            List<Vector2Int> barracksAtMaxLevel = new List<Vector2Int>();
            foreach (var pos in barracks)
            {
                if (Gm.gameBoard.WorldMap[pos].ParentChunk.Level > maxlevel)
                    maxlevel = Gm.gameBoard.WorldMap[pos].ParentChunk.Level;
                    barracksAtMaxLevel.Clear();
                    barracksAtMaxLevel.Add(pos);
                if(Gm.gameBoard.WorldMap[pos].ParentChunk.Level==maxlevel)
                    barracksAtMaxLevel.Add(pos);
            }
                
            rand = Random.Range(0, barracksAtMaxLevel.Count);
            return (Building.Barrack,barracksAtMaxLevel[rand]);
        }
    }
}