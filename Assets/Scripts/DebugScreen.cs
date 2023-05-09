using System;

using UnityEngine;
using UnityEngine.UI;

public class DebugScreen : MonoBehaviour
{
    [SerializeField]
    private GameObject hexTile;
    
    [SerializeField]
    private Text fps;
    private float _dt;

    [SerializeField]
    private GameObject gui;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
            gui.SetActive(!gui.activeSelf);
        
        _dt += (Time.deltaTime - _dt) * 0.1f;
        fps.text =$@"{Mathf.Ceil (1.0f / _dt)} fps
Press D to hide this panel
Press Enter to go to next phase
Press Numpad enter to go to next player
Press Munpad + / - to zoom in / out
Press Numpad 1 / 2 to go to phase 1 / 2
Press Escape to go to the menu";
    }
}