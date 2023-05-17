using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace UI
{
    public class VictoryMgr : MonoBehaviourMgr<VictoryMgr>
    {
        public void Start()
        {
            this.gameObject.SetActive(false);
        }

        public void Restart() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        public void MainMenu() => throw new NotImplementedException();

        public void Quit() =>
#if UNITY_EDITOR
        Debug.Break();
#else
        Application.Quit();
#endif

    }
}
