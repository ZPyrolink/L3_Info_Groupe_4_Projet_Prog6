using System;

using UI;

using UnityEngine;

public class MenuMgr : MonoBehaviour
{
    public void Continue() => FindObjectOfType<UiMgr>().ToggleMenu();
    public void Save() => throw new NotImplementedException();
    public void Restart() => throw new NotImplementedException();
    public void Quit() =>
#if UNITY_EDITOR
        Debug.Break();
#else
        Application.Quit();
#endif
}