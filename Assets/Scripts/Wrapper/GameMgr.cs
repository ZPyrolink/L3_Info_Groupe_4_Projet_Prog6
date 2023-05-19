using System;
using System.Linq;

using Taluva.Controller;
using Taluva.Model;
using Taluva.Model.AI;

using UI;

using UnityEngine;

using Utils;

using Random = UnityEngine.Random;

namespace Wrapper
{
    public class GameMgr : MonoBehaviourMgr<GameManagment>
    {
        [SerializeField]
        private sbyte nbPlayers, nbAis;

        [SerializeField]
        private string path;

        [SerializeField]
        private bool load;

        protected override GameManagment InitInstance
        {
            get
            {
                if (string.IsNullOrEmpty(Settings.LoadedFile))
                    Settings.LoadedFile = path;
                else
                    load = true;

                if (load)
                {
                    GameManagment gm = new(2);
                    return gm;
                }

                if (Settings.PlayerNb == 0)
                    Settings.PlayerNb = nbPlayers;

                if (Settings.AiNb == 0)
                    Settings.AiNb = nbAis;

                return new(Settings.PlayerNb, Enumerable.Repeat(typeof(AIRandom), Settings.AiNb).ToArray());
            }
        }

        private void Start()
        {
            SetHandlers();
            if(load)
                Instance.LoadGame(Settings.LoadedFile);
            if (Instance.gameBoard.WorldMap.Empty)
                Instance.InitPlay();
            else if (Instance.actualPhase == TurnPhase.PlaceBuilding)
                Instance.InitPlay(false, false);
            else
                Instance.InitPlay(true, false);
        }

        private void SetHandlers()
        {
            Instance.ChangePhase = phase =>
            {
                //Debug.Log($"Change Phase into {phase}");
                UiMgr ui = UiMgr.Instance;

                (phase switch
                {
                    TurnPhase.SelectCells => (Action)ui.Phase1,
                    TurnPhase.PlaceBuilding => ui.Phase2,
                    _ => () => Debug.LogWarning($"The {Instance.actualPhase} is not implemented!")
                }).Invoke();
            };

            Instance.NotifyEndGame = (player, end) => 
            { 
                VictoryMgr.Instance.SetWinnerText(player.ID.ToString());
                UiMgr.Instance.ToggleMenu();
            };

            Instance.NotifyPlayerEliminated = player =>
            { //Debug.Log(player);
            };

            Instance.NotifyReputTile = (pos, r) => 
            { 
                TilesMgr.Instance.ReputTile(pos, r); 
            };

            Instance.NotifyReputBuild = (pos, b) =>
            {
                TilesMgr.Instance.ReputBuild(pos, b);
            };

            Instance.NotifyAIBuildingPlacement = (building, i) => throw new NotImplementedException();
            Instance.NotifyAIChunkPlacement = rotation => throw new NotImplementedException();
        }
    }
}