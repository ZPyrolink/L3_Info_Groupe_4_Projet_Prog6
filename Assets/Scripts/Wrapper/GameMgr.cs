using System;

using Taluva.Controller;
using Taluva.Model;

using UI;

using UnityEngine;

namespace Wrapper
{
    public class GameMgr : MonoBehaviour
    {
        [SerializeField]
        private int nbPlayers;

        public static GameManagment Instance;

        private void Awake()
        {
            Instance = new(nbPlayers);
        }

        private void Start()
        {
            SetHandlers();
            Instance.InitPlay();
        }

        private void SetHandlers()
        {
            Instance.ChangePhase = phase =>
            {
                UiMgr ui = UiMgr.Instance;

                (phase switch
                {
                    TurnPhase.SelectCells => (Action) ui.Phase1,
                    TurnPhase.PlaceBuilding => ui.Phase2,
                    _ => () => Debug.LogWarning($"The {Instance.actualPhase} is not implemented!")
                }).Invoke();
            };

            Instance.NotifyEndGame = (player, end) => { Debug.Log(player + " " + end); };

            Instance.NotifyPlayerEliminated = player => { Debug.Log(player); };

            Instance.NotifyAIBuildingPlacement = (building, i) => throw new NotImplementedException();
            Instance.NotifyAIChunkPlacement = rotation => throw new NotImplementedException();
        }
    }
}