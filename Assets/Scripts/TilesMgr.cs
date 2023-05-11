﻿using System;
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

    [SerializeField]
    private GameObject barrack, tower, temple;

    private Building _currentBuild;

    private Dictionary<Building, Vector3> _buildsScale = new()
    {
        [Building.Barrack] = new(23, 27, 15),
        [Building.Tower] = new(120, 120, 29),
        [Building.Temple] = new(38, 27, 23)
    };

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

        if (!Physics.Raycast(ray, out RaycastHit hit, float.PositiveInfinity,
                LayerMask.GetMask("Feed Forward")))
        {
            Debug.Log("Don't touch anything!");
            return;
        }

        switch (hit.transform.tag)
        {
            // case "Tile":
            //     OutlineGo(hit.transform.gameObject);
            //     break;
            case "Feed Forward":
                _currentFf = hit.transform.gameObject;
                (UiMgr.Instance.Phase switch
                {
                    1 => (Action<Vector3>) PutTile,
                    2 => PutBuild
                }).Invoke(hit.transform.position);
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
        Chunk c = new(level, left, right);
        _board.AddChunk(c, new(PlayerColor.Blue), new(pos, rot), rot);
        
        ClearFeedForward();
    }

    private void PutBuild(Vector3 pos)
    {
        if (_current is null)
        {
            _current = Instantiate(_currentBuild switch
            {
                Building.Barrack => barrack,
                Building.Tower => tower,
                Building.Temple => temple
            }, boardParent);
            
            _current.transform.localScale = _buildsScale[_currentBuild];
            Material[] mats = _current.GetComponent<MeshRenderer>().materials;
            foreach (Material mat in mats)
            {
                mat.SetRenderMode(MaterialExtensions.BlendMode.Transparent);
                mat.color = PlayerMgr.Instance.Current.Color.With(a: .75f);
            }
        }

        if (_current.transform.position == pos)
            return;
        
        _current.transform.position = pos;
    }

    public void ValidateBuild()
    {
        Material[] mats = _current.GetComponent<MeshRenderer>().materials;
        foreach (Material mat in mats.Where((_, i) => i != 1))
        {
            mat.SetRenderMode(MaterialExtensions.BlendMode.Opaque);
            mat.color = mat.color.With(a: 1);
        }
        
        Vector2Int pos = new() { x = (int) (_current.transform.position.x / xOffset) };
        if (pos.x % 2 != 0)
            pos.y = (int) ((_current.transform.position.z - zOffset / 2) / zOffset);
        else
            pos.y = (int) (_current.transform.position.z / zOffset);
        
        Debug.Log((pos, (int) (_current.transform.position.y / yOffset)));
        
        _board.PlaceBuilding(_board.WorldMap[pos], _currentBuild, new(PlayerColor.Blue));
        
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
        ClearFeedForward();
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

    public void SetFeedForwards2(Building build)
    {
        ClearFeedForward();
        _currentBuild = build;
        Vector2Int[] poss = build switch
        {
            Building.Barrack => _board.GetBarrackSlots(),
            Building.Tower => _board.GetTowerSlots(new(PlayerColor.Blue)),
            Building.Temple => _board.GetTempleSlots(new(PlayerColor.Blue))
        };

        foreach (Vector2Int p in poss)
        {
            Vector3 pos = new(p.x, 0, p.y);
            if (!_board.WorldMap.IsVoid(p))
                pos.y = _board.WorldMap[p].ParentCunk.Level * yOffset;
            pos.Scale(new(xOffset, 1, zOffset));
            if (p.x % 2 != 0)
                pos.z += zOffset / 2;
            _gos[SetFeedForward(pos)] = new(p);
        }
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
        _current = null;
        foreach (Transform t in feedForwardParent)
            Destroy(t.gameObject);
    }

    public void ClearOutline()
    {
        foreach (Outline o in GameObject.FindGameObjectsWithTag("Tile").Select(go => go.GetComponent<Outline>()))
            o.enabled = false;
    }
}