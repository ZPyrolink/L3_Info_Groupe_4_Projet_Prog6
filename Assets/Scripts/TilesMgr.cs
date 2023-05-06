using System.Linq;

using Imports.QuickOutline.Scripts;

using UnityEngine;

public class TilesMgr : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform.CompareTag("Tile"))
                OutlineGo(hit.transform.gameObject);
        }
    }

    private void OutlineGo(GameObject go)
    {
        ClearOutline();

        Outline outline = go.GetComponent<Outline>();
        outline.enabled = true;
        outline.OutlineColor = PlayerMgr.Instance.Current.Color;
        outline.OutlineMode = Outline.Mode.OutlineAll;
    }

    public void ClearOutline()
    {
        foreach (Outline o in GameObject.FindGameObjectsWithTag("Tile").Select(go => go.GetComponent<Outline>()))
            o.enabled = false;
    }
}