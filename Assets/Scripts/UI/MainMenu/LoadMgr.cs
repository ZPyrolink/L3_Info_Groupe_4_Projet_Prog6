using System.IO;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.MainMenu
{
    public class LoadMgr : MonoBehaviour
    {
        [SerializeField]
        private Button[] slots;

        private void Start()
        {
            Debug.Log(Directory.GetCurrentDirectory());
            for (int i = 1; i <= slots.Length; i++)
                slots[i - 1].interactable = File.Exists(Directory.GetCurrentDirectory() + "/Save/" + $"slot{i}");
        }

        public void LoadGame(int i)
        {
            Settings.LoadedFile = $"slot{i}";
            SceneManager.LoadScene("SampleScene");
        }
    }
}