using System;
using System.Collections.Generic;
using System.Linq;

using Imports.QuickOutline.Scripts;

using Taluva.Model;

using UI;

using UnityEngine;

using Utils;

public class TilesMgr : MonoBehaviour
{
    private const float xOffset = 1.5f, yOffset = .41f, zOffset = 1.73205f;
    public static TilesMgr Instance { get; private set; }

    private Board _board;

    [SerializeField]
    private Transform boardParent;

    private GameObject _current, _currentFf;

    [SerializeField]
    private GameObject feedForward;

    [SerializeField]
    private Transform feedForwardParent;

    private Dictionary<GameObject, PointRotation> _gos;

    private void Start()
    {
        Instance = this;
        _board = new();
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log("Don't touch anything!");
            return;
        }

        if (hit.transform == _current?.transform)
        {
            RotateTile();
            return;
        }

        switch (hit.transform.tag)
        {
            case "Tile":
                OutlineGo(hit.transform.gameObject);
                break;
            case "Feed Forward":
                _currentFf = hit.transform.gameObject;
                PutTile(hit.transform.position);
                break;

            default:
                Debug.Log("Another object is hited", hit.transform);
                break;
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

    private void PutTile(Vector3 pos)
    {
        if (_current is null)
        {
            GameObject tmp = UiMgr.Instance.CurrentTile;
            _current = Instantiate(tmp, boardParent);
            tmp.SetActive(false);
            _current.transform.localScale = new(100, 100, 100);
            _current.layer = LayerMask.NameToLayer("Default");
            Material[] mats = _current.GetComponent<MeshRenderer>().materials;
            foreach (Material mat in mats.Where((_, i) => i != 1))
            {
                mat.SetRenderMode(MaterialExtensions.BlendMode.Transparent);
                mat.color = mat.color.With(a: .75f);
            }
        }

        if (_current.transform.position == pos)
            RotateTile();
        else
        {
            _current.transform.position = pos;
            _current.transform.rotation = Quaternion.Euler(270,
                _gos == null ? 270 : ((Rotation) Array.IndexOf(_gos[_currentFf].rotations, true)).YDegree(),
                0);
        }
    }

    public void ValidateTile()
    {
        Material[] mats = _current.GetComponent<MeshRenderer>().materials;
        foreach (Material mat in mats.Where((_, i) => i != 1))
        {
            mat.SetRenderMode(MaterialExtensions.BlendMode.Opaque);
            mat.color = mat.color.With(a: 1);
        }

        Cell left = new(BiomeColorExt.Of(mats[0].color)), right = new(BiomeColorExt.Of(mats[3].color));

        (Vector2Int pos, Rotation rot, int level) = GetPr();
        Debug.Log($"Validate tile on {pos} with {rot} rotation at level {level}");
        Chunk c = new(level, left, right);
        _board.AddChunk(c, new(PlayerColor.Blue), new(pos, rot), rot);

        _current = null;
        ClearFeedForward();
    }

    private (Vector2Int pos, Rotation rot, int level) GetPr()
    {
        Rotation rot = RotationExt.Of(Mathf.Round(_current.transform.rotation.eulerAngles.y));
        Vector2Int pos = new() { x = (int) (_current.transform.position.x / xOffset) };
        if (pos.x % 2 != 0)
            pos.y = (int) ((_current.transform.position.z - zOffset / 2) / zOffset);
        else
            pos.y = (int) (_current.transform.position.z / zOffset);

        return (pos, rot, (int) (_current.transform.position.y / yOffset) + 1);
    }

    private void RotateTile()
    {
        Rotation rot;

        do
        {
            _current.transform.Rotate(new(0, 360f / 6, 0), Space.World);
            rot = RotationExt.Of(Mathf.Round(_current.transform.rotation.eulerAngles.y));
        } while (_gos?[_currentFf]?.rotations?[(int) rot] == false);
    }

    public void SetFeedForwards1()
    {
        _gos = new();
        foreach (PointRotation pr in _board.GetChunkSlots())
        {
            Vector3 pos = new(pr.point.x, 0, pr.point.y);
            if (!_board.WorldMap.IsVoid(pr.point))
                pos.y = _board.WorldMap[pr.point].ParentCunk.Level * yOffset;
            pos.Scale(new(xOffset, 1, zOffset));
            if (pr.point.x % 2 != 0)
                pos.z += zOffset / 2;
            _gos[SetFeedForward(pos)] = pr;
        }
    }

    public void SetFeedForwards2()
    {
        // foreach (Vector2Int pos in _board.GetBarrackSlots())
        //     print(pos);
        //
        // foreach (Vector2Int pos in _board.GetTowerSlots(new(PlayerColor.Blue)))
        //     print(pos);
        //
        // foreach (Vector2Int pos in _board.GetTempleSlots(new(PlayerColor.Blue)))
        //     print(pos);
    }

    public GameObject SetFeedForward(Vector3 pos)
    {
        GameObject go = Instantiate(feedForward, pos, Quaternion.Euler(-90, -90, 0), feedForwardParent);
        go.GetComponent<MeshRenderer>().materials[1].color = PlayerMgr.Instance.Current.Color;
        return go;
    }

    public void ClearFeedForward()
    {
        _gos = new();
        foreach (Transform t in feedForwardParent)
            Destroy(t.gameObject);
    }

    public void ClearOutline()
    {
        foreach (Outline o in GameObject.FindGameObjectsWithTag("Tile").Select(go => go.GetComponent<Outline>()))
            o.enabled = false;
    }
}