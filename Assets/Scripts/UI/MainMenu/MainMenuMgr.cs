using Taluva.Model;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenu
{
    public class MainMenuMgr : MonoBehaviour
    {
        private const string DISCORD_URL = "https://discordapp.com/users/";
        
        [SerializeField]
        private string[] userIds;
        
        [SerializeField]
        private GameObject contactUs,
            main,
            play,
            load,
            settings;

        [SerializeField]
        private Toggle audioToggle;
        [SerializeField]
        private AudioSource audioSource;

        public void Start()
        {
            audioToggle.isOn = Settings.allowSound;
            audioSource.mute = !Settings.allowSound;
        }

        public void OpenSettings() { }

        public void TogglePlay()
        {
            ToggleMain();
            Toggle(play);
        }

        public void ToggleLoad()
        {
            ToggleMain();
            Toggle(load);
        }

        public void ToggleSettings()
        {
            ToggleMain();
            Toggle(settings);
        }

        public void AllowSound()
        {
            audioSource.mute = !audioToggle.isOn;
            Settings.allowSound = audioToggle.isOn;
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
            Toggle(contactUs);
        }

        public void ToggleMain() => Toggle(main);

        private void Toggle(GameObject go) => go.SetActive(!go.activeSelf);

        public void Contact(int i) => Application.OpenURL(DISCORD_URL + userIds[i]);
    }
}