
using System;
using System.Collections.Generic;
using System.Linq;
using Taluva.Model;
using Taluva.Model.AI;
using UI;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.SceneManagement;
using Wrapper;

public class MenuMgr : MonoBehaviour
{
    public void Continue() => FindObjectOfType<UiMgr>().ToggleMenu();
    public void Save() => SaveMgr.Instance.ToggleMenu();
    public void Restart() => ResetGame();
    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit() =>
#if UNITY_EDITOR
        Debug.Break();
#else
        Application.Quit();
#endif
}