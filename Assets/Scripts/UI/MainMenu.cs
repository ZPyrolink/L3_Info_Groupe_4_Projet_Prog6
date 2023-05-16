using UnityEngine;

namespace UI
{
    public class MainMenu : MonoBehaviour
    {
        private const string DISCORD_URL = "https://discordapp.com/users/";
        
        [SerializeField]
        private string[] userIds;
        
        [SerializeField]
        private GameObject contactUs,
            main,
            play;
        
        public void OpenSettings() { }

        public void TogglePlay(bool ai)
        {
            ToggleMain();
            play.SetActive(!play.activeSelf);
        }

        public void Rules() { }

        public void Exit() =>
#if UNITY_EDITOR
            Debug.Break();
#else
            Application.Quit();
#endif

        public void ToggleContactUs()
        {
            ToggleMain();
            contactUs.SetActive(!contactUs.activeSelf);
        }

        public void ToggleMain() => main.SetActive(!main.activeSelf);

        public void Contact(int i) => Application.OpenURL(DISCORD_URL + userIds[i]);
    }
}