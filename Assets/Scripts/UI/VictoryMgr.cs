using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;

namespace UI
{
    public class VictoryMgr : MonoBehaviourMgr<VictoryMgr>
    {
        public void Start()
        {
            this.gameObject.SetActive(false);
        }

        [SerializeField]
        private Text text;

        public void SetWinnerText(string player) => text.text = "The player " + player + " win"; 

        public void Restart() => ResetGame();
        public void ResetGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void MainMenu() => SceneManager.LoadScene("MainMenu");

        public void Quit() =>
#if UNITY_EDITOR
        Debug.Break();
#else
        Application.Quit();
#endif

    }
}
