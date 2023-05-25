using System.Collections.Generic;
using System.Linq;
using Taluva.Model;
using Taluva.Model.AI;
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

            Settings.Name = new string[StartSettings.PlayerNb - StartSettings.Ais.Length];
            int index = 0;
            for(int i = 0;  i < StartSettings.PlayerNb; i++)
            {
                if (pss[i].playerType.value == 0)
                {
                    Settings.Name[index++] = pss[i].PlayerName.text;
                }

            }

            SceneManager.LoadScene("SampleScene");
        }
    }
}