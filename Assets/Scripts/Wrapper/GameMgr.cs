using System;

using Taluva.Controller;
using Taluva.Model;

using UI;

using UnityEngine;

using Utils;

namespace Wrapper
{
    public class GameMgr : MonoBehaviourMgr<GameManagment>
    {
        [SerializeField]
        private int nbPlayers;

        protected override GameManagment InitInstance => new(nbPlayers);

        private void Start()
        {
            SetHandlers();
            Instance.InitPlay();
        }

        private void SetHandlers()
        {
            Instance.ChangePhase = phase =>
            {
                //Debug.Log($"Change Phase into {phase}");
                UiMgr ui = UiMgr.Instance;

                (phase switch
                {
                    TurnPhase.SelectCells => (Action) ui.Phase1,
                    TurnPhase.PlaceBuilding => ui.Phase2,
                    _ => () => Debug.LogWarning($"The {Instance.actualPhase} is not implemented!")
                }).Invoke();
            };

            Instance.NotifyEndGame = (player, end) => { Debug.Log(player + " " + end); };

            Instance.NotifyPlayerEliminated = player => { //Debug.Log(player);
                                                          };

            Instance.NotifyAIBuildingPlacement = (building, i) => throw new NotImplementedException();
            Instance.NotifyAIChunkPlacement = rotation => throw new NotImplementedException();
        }
    }
}