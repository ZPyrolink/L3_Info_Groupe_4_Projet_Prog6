using System;

using Imports.QuickOutline.Scripts;

using UnityEngine;

public class ClickMgr : MonoBehaviour
{
    private Outline _current;
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
                OutlineTile(hit.transform.gameObject);
        }
    }

    private void OutlineTile(GameObject go)
    {
        if (_current is not null)
        {
            if (_current.gameObject == go)
                return;
            
            Destroy(_current);
        }
        _current = go.AddComponent<Outline>();
        _current.OutlineColor = Color.cyan;
        _current.OutlineWidth = .5f;
    }
}