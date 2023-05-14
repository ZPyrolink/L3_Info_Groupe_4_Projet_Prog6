using System;
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

    private Plane _horizontalAxisPlane = new(Vector3.up, Vector3.zero);

    private Vector3 _defaultPosition;
    private Quaternion _defaultRotation;

    private int _currentRotation;
    
    // Start is called before the first frame update
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        _cam = Camera.main;

        _defaultPosition = _cam.transform.position;
        _defaultRotation = _cam.transform.rotation;

        _currentRotation = 0;
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

        if (Input.GetMouseButton(1))
            RightClickMove(mouseMove);
        // else
        //     OutsideWindowMove(mouseMove);
    }

    private Vector3 ScreenToHorizontalPlane(Vector3 origin, Vector3 direction)
    {
        _horizontalAxisPlane.Raycast(new(origin, direction), out float distance);
        return origin + direction * distance;
    }

    private void RightClickMove(Vector2 mouseMove) => Move(-mouseMove);

    // private void OutsideWindowMove(Vector2 mouseMove)
    // {
    //     Vector2 mousePosition = Input.mousePosition;
    //     Vector2 screenSize = new(Screen.width, Screen.height);
    //
    //     Vector3 movement = new();
    //     if (mousePosition.x <= screenDetectionOffset && mouseMove.x < 0 ||
    //         mousePosition.x >= screenSize.x - screenDetectionOffset && mouseMove.x > 0)
    //     {
    //         movement.x = mouseMove.x;
    //     }
    //
    //     if (mousePosition.y <= screenDetectionOffset && mouseMove.y < 0 ||
    //         mousePosition.y >= screenSize.y - screenDetectionOffset && mouseMove.y > 0)
    //     {
    //         movement.y = mouseMove.y;
    //     }
    //
    //     Move(movement);
    // }

    private void Move(Vector3 direction) => _cam.transform.Translate(direction * moveFactor);
    
    public void Rotate(float angle)
    {
        Transform camTr = _cam.transform;
        _cam.transform.RotateAround(ScreenToHorizontalPlane(camTr.position, camTr.forward),
            Vector3.up, angle);

        _currentRotation++;
        _currentRotation %= rotationNb;
    }

    public void ResetPosition()
    {
        _cam.transform.position = _defaultPosition;
        _cam.transform.rotation = _defaultRotation;
        
        for (int i = 0; i < _currentRotation; i++)
            Rotate(360f / rotationNb);
    }

    public void Zoom(int factor)
    {
        float tmp = _cam.orthographicSize;
        tmp -= factor;
        if (tmp < 5)
            tmp = 5;
        if (tmp > 50)
            tmp = 50;
        _cam.orthographicSize = tmp;
    }
}