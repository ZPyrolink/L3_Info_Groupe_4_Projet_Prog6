using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.MainMenu
{
    public class PlayMenu : MonoBehaviour
    {
        [SerializeField]
        private Text nbPlayers,
            nbAi;

        [SerializeField]
        private Slider nbPlayersSlider,
            nbAiSlider;

        [SerializeField]
        private string nbPlayersPlaceholder,
            nbAiPlaceholder;

        private void Start()
        {
            if (string.IsNullOrEmpty(nbPlayersPlaceholder))
                nbPlayersPlaceholder = nbPlayers.text;

            if (string.IsNullOrEmpty(nbAiPlaceholder))
                nbAiPlaceholder = nbAi.text;
            
            OnPlayerNbChanged(nbPlayersSlider.value);
            OnAiNbChanged(nbAiSlider.value);
        }

        public void OnPlayerNbChanged(float i)
        {
            nbPlayers.text = nbPlayersPlaceholder.Replace("%nb%", i.ToString());
            nbAiSlider.maxValue = i;
        }

        public void OnAiNbChanged(float i) =>
            nbAi.text = nbAiPlaceholder.Replace("%nb%", i.ToString());

        public void Play()
        {
            Settings.PlayerNb = (sbyte) nbPlayersSlider.value;
            Settings.AiNb = (sbyte) nbAiSlider.value;

            SceneManager.LoadScene("SampleScene");
        }
    }
}