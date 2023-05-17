using System;
using System.Collections.Generic;
using System.Linq;
using Taluva.Controller;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Taluva.Model.AI
{
    public class AIMonteCarlo : AITree
    {
        public AIMonteCarlo(PlayerColor id, GameManagment gm) : base(id, gm)
        {
        }

        public AIMonteCarlo(AIMonteCarlo original) : base(original)
        {
        }

        public override Player Clone()
        {
            return new AIMonteCarlo(this);
        }
        

        protected override int Heuristic
        {
            get
            {
                int val = 0;
                GameManagment virtualGM = new GameManagment(gm);
                for (int i = 0; i < 500; i++)
                {
                    while (true) //TODO condition go on till the games end.
                    {
                        //AIPlayChunk(PlayRandomChunk(gm.gameBoard));
                        //AIPlayBuilding(PlayRandomBuild(gm.gameBoard));

                    }
                    if (true) // Si l'IA gagne;
                    {
                        val++;
                    }
                }
                return val;
            }
        }

        protected override GameManagment.Coup GetBest(Dictionary<int,GameManagment.Coup> possible)
        {
            return possible.Max().Value;
            throw new System.NotImplementedException();
        }
        
        public PointRotation PlayRandomChunk(Board board)
        {
            int rand;
            int max = 0;
            PointRotation[] possible = board.GetChunkSlots();
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

        public (Building buil, Vector2Int pos) PlayRandomBuild(Board board)
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
            Vector2Int[] barracks = board.GetBarrackSlots(this);
            rand = Random.Range(0, barracks.Length);
            return (Building.Barrack,barracks[rand]);
        }
    }
    
    
    
    
    
    
}