using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Taluva.Controller;
using Taluva.Model;
using Taluva.Model.AI;

using UI;

using UnityEngine;

using Utils;

namespace Wrapper
{
    public class GameMgr : MonoBehaviourMgr<GameManagment>
    {
        [SerializeField]
        private sbyte nbPlayers;

        [SerializeField]
        private Difficulty[] ais;

        [SerializeField]
        private string path;

        [SerializeField]
        public bool load;

        private Queue<AiMove> _aiMoves;
        private TurnPhase _nextPhase;
        private bool _AiWorking => _aiMoves.Count != 0;

        private bool _coroutineStarted;

        protected override GameManagment InitInstance
        {
            get
            {
                if (string.IsNullOrEmpty(StartSettings.LoadedFile))
                    StartSettings.LoadedFile = path;
                else
                    load = true;


                if (load)
                {
                    GameManagment gm = new(4);
                    return gm;
                }

                if (StartSettings.PlayerNb == 0)
                    StartSettings.PlayerNb = nbPlayers;

                if (StartSettings.Ais == null)
                    StartSettings.Ais = ais;

                return new(StartSettings.PlayerNb, StartSettings.Ais.Select(d => d switch
                {
                    Difficulty.BadPlayer => typeof(AIRandom),
                    Difficulty.Normal => typeof(AIMonteCarlo)
                }).ToArray());
            }
        }

        private void Start()
        {
            _aiMoves = new();
            SetHandlers();
            if (load)
            {
                Instance.LoadGame(StartSettings.LoadedFile);
                UiMgr.Instance.UnloadSetUp();
            }
            
            if (Instance.gameBoard.WorldMap.Empty)
                Instance.InitPlay();
            else if (Instance.actualPhase == TurnPhase.PlaceBuilding)
                Instance.InitPlay(false, false);
            else
                Instance.InitPlay(true, false);
        }

        private void SetHandlers()
        {
            Instance.ChangePhase = ChangePhase;

            Instance.NotifyEndGame = (player, end) => 
            { 
                VictoryMgr.Instance.SetWinnerText(player.ID.ToString());
                UiMgr.Instance.ToggleVictory();
            };

            Instance.NotifyPlayerEliminated = player =>
            {
                //Debug.Log(player);
            };
            
            Instance.NotifyReputTile = (pos, r) => 
            { 
                TilesMgr.Instance.ReputTile(pos, r); 
            };

            Instance.NotifyReputBuild = (pos, b) =>
            {
                TilesMgr.Instance.ReputBuild(pos, b, Instance.actualPlayer);
            };

            Instance.NotifyAIChunkPlacement = pr =>
            {
                Vector3 p = new(pr.point.x, 0, pr.point.y);
                if (!Instance.IsVoid(pr.point))
                    p.y = Instance.LevelAt(pr.point) * TilesMgr.yOffset;
                p.Scale(new(TilesMgr.xOffset, 1, TilesMgr.zOffset));

                if (pr.point.x % 2 != 0)
                    p.z += TilesMgr.zOffset / 2;
                
                _aiMoves.Enqueue(new(pr.point, p, (Rotation) Array.IndexOf(pr.rotations, true),
                    Instance.actualChunk));
                if (!_coroutineStarted)
                    StartCoroutine(CTemporateAi());
            };

            Instance.NotifyAIBuildingPlacement = (building, pos) =>
            {
                _aiMoves.Enqueue(new(pos, building));
                if (!_coroutineStarted)
                    StartCoroutine(CTemporateAi());
            };
        }
        
        private void ChangePhase(TurnPhase phase)
        {
            if (_AiWorking)
            {
                _nextPhase = phase;
                return;
            }

            UiMgr ui = UiMgr.Instance;

            (phase switch
            {
                TurnPhase.SelectCells => (Action) ui.Phase1,
                TurnPhase.PlaceBuilding => ui.Phase2,
                _ => () => Debug.LogWarning($"The {Instance.actualPhase} is not implemented!")
            }).Invoke();
        }

        private IEnumerator CTemporateAi()
        {
            _coroutineStarted = true;
            do
            {
                AiMove aiMove = _aiMoves.Dequeue();
                yield return new WaitForSeconds(1);
                switch (aiMove.Turn)
                {
                    case TurnPhase.SelectCells:
                        UiMgr.Instance.UpdateTiles();
                        TilesMgr.Instance.PutAiTile(aiMove.BoardPos, aiMove.GamePos, aiMove.Rot, aiMove.Chunk);
                        Instance.AiBuild();
                        break;
                    case TurnPhase.PlaceBuilding:
                        TilesMgr.Instance.ReputBuild(aiMove.BoardPos, aiMove.Build, Instance.actualPlayer);
                        Instance.ContinueAi();
                        break;
                }
            } while (_aiMoves.Count != 0);

            ChangePhase(_nextPhase);
            _coroutineStarted = false;
        }

        private class AiMove
        {
            public TurnPhase Turn { get; }
            public Vector2Int BoardPos { get; }
            
            public Vector3 GamePos { get; }
            public Rotation Rot { get; }
            public Chunk Chunk { get; }

            public Building Build { get; }

            public AiMove(Vector2Int boardPos, Vector3 gamePos, Rotation rot, Chunk chunk)
            {
                Turn = TurnPhase.SelectCells;
                BoardPos = boardPos;
                GamePos = gamePos;
                Rot = rot;
                Chunk = chunk;
            }

            public AiMove(Vector2Int boardPos, Building build)
            {
                Turn = TurnPhase.PlaceBuilding;
                Build = build;
                BoardPos = boardPos;
            }
        }
    }
}