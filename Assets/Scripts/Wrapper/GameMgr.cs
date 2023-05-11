using System;

using Taluva.Controller;

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
            Instance.InitPlay();
        }
    }
}