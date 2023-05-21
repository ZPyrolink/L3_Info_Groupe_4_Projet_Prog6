using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.MainMenu
{
    public class PlayMenu : MonoBehaviour
    {
        [SerializeField]
        private GameObject playersSelectionParent;

        public void Play()
        {
            PlayerSelection[] pss = playersSelectionParent.GetComponentsInChildren<PlayerSelection>();
            StartSettings.PlayerNb = (sbyte) pss.Count(ps => ps.Registered);
            StartSettings.Ais = pss
                // On ignore les joueurs non enregistrés ou humain
                .Where(ps => ps.Registered && ps.Ai >= 0)
                // On récupère la difficulté
                .Select(ps => ps.Ai).ToArray();

            SceneManager.LoadScene("SampleScene");
        }
    }
}