using System;

using Taluva.Model;

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

    private void Start()
    {
        Debug.Log("Debug enabled!");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
            gui.SetActive(!gui.activeSelf);

        _dt += (Time.deltaTime - _dt) * 0.1f;
        fps.text = $@"{Mathf.Ceil(1.0f / _dt)} fps";
        
        
    }
}