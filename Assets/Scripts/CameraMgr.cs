using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class CameraMgr : MonoBehaviour
{
    private Camera _cam;

    [SerializeField]
    private int rotationNb = 6;

    [SerializeField]
    private float moveFactor = 1;

    [SerializeField]
    private float screenDetectionOffset = 5.5f;

    [Header("Debug keys")]
    [SerializeField]
    private KeyCode zoomIn = KeyCode.KeypadPlus;

    [SerializeField]
    private KeyCode zoomOut = KeyCode.KeypadMinus;

    // Start is called before the first frame update
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        _cam = Camera.main;
        Debug.Log($"{Screen.width}, {Screen.height}");
    }

    private void Update()
    {
        if (Input.GetKeyDown(zoomIn))
            Zoom(5);

        if (Input.GetKeyDown(zoomOut))
            Zoom(-5);

        if (Input.GetKeyDown(KeyCode.Space))
            Rotate(360f / rotationNb);

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            Move(Vector3.left);

        if (Input.GetKeyDown(KeyCode.RightArrow))
            Move(Vector3.right);

        if (Input.GetKeyDown(KeyCode.UpArrow))
            Move(Vector3.up);

        if (Input.GetKeyDown(KeyCode.DownArrow))
            Move(Vector3.down);

        Vector3 mouseMove = new(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse ScrollWheel"));

        switch (mouseMove.z)
        {
            case > 0:
                Zoom(5);
                break;
            case < 0:
                Zoom(-5);
                break;
        }

        CheckBorderMove(mouseMove);
    }

    private void CheckBorderMove(Vector2 mouseMove)
    {
        if (mouseMove == Vector2.zero)
            return;

        Vector2 mousePosition = Input.mousePosition;
        Vector2 screenSize = new(Screen.width, Screen.height);
        
        Debug.Log(mousePosition.x);
        Debug.Log(screenSize.x - screenDetectionOffset);
        Debug.Log(mousePosition.x >= screenSize.x - screenDetectionOffset);
        Debug.Log(mouseMove.x > 0);

        Vector3 movement = new();
        if (mousePosition.x <= screenDetectionOffset && mouseMove.x < 0 ||
            mousePosition.x >= screenSize.x - screenDetectionOffset && mouseMove.x > 0)
        {
            movement.x = mouseMove.x;
        }

        if (mousePosition.y <= screenDetectionOffset && mouseMove.y < 0 ||
            mousePosition.y >= screenSize.y - screenDetectionOffset && mouseMove.y > 0)
        {
            movement.y = mouseMove.y;
        }

        Move(movement);
    }

    private void Move(Vector3 direction) => _cam.transform.Translate(direction * moveFactor);

    public void Rotate(float angle)
    {
        _cam.transform.RotateAround(Vector3.zero, Vector3.up, angle);
    }

    public void Zoom(int factor)
    {
        float tmp = _cam.orthographicSize;
        tmp -= factor;
        if (tmp < 5)
            tmp = 5;
        _cam.orthographicSize = tmp;
    }
}