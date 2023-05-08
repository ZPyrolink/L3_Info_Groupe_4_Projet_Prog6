using System;

using UI;

using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuMgr : MonoBehaviour
{
    public void Continue() => FindObjectOfType<UiMgr>().ToggleMenu();
    public void Save() => throw new NotImplementedException();
    public void Restart() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    public void Quit() =>
#if UNITY_EDITOR
        Debug.Break();
#else
        Application.Quit();
#endif
}