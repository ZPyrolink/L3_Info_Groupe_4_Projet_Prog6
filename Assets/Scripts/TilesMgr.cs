using System;
using System.Collections.Generic;
using System.Linq;

using Taluva.Model;
using Taluva.Model.AI;
using UI;

using UnityEngine;

using Utils;

using Wrapper;

public class TilesMgr : MonoBehaviourMgr<TilesMgr>
{
    public const float xOffset = 1.5f, yOffset = .41f, zOffset = 1.73205f;

    [SerializeField]
    private Transform boardParent;

    private GameObject _currentFf;
    private GameObject[] _currentPreviews;

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
            case "Feed Forward":
                _currentFf = hit.transform.gameObject;
                (GameMgr.Instance.actualPhase switch
                {
                    TurnPhase.SelectCells => (Action<Vector3>) PutTile,
                    TurnPhase.PlaceBuilding => PutBuild
                }).Invoke(hit.transform.position);
                break;

            default:
                Debug.Log("Another object is hited", hit.transform);
                break;
        }
    }

    private void PutTile(Vector3 pos)
    {
        if (_currentPreviews is null)
        {
            GameObject tmp = UiMgr.Instance.CurrentTile;
            _currentPreviews = new[] { Instantiate(tmp, boardParent) };
            tmp.SetActive(false);
            _currentPreviews[0].transform.localScale = new(100, 100, 100);
            _currentPreviews[0].layer = LayerMask.NameToLayer("Default");
            Material[] mats = _currentPreviews[0].GetComponent<MeshRenderer>().materials;
            foreach (Material mat in mats.Where((_, i) => i != 1))
            {
                mat.SetRenderMode(MaterialExtensions.BlendMode.Transparent);
                mat.color = mat.color.With(a: .75f);
            }
        }

        if (_currentPreviews[0].transform.position == pos)
            RotateTile();
        else
        {
            _currentPreviews[0].transform.position = pos;
            _currentPreviews[0].transform.rotation = Quaternion.Euler(270,
                _gos == null ? 270 : ((Rotation) Array.IndexOf(_gos[_currentFf].rotations, true)).YDegree(),
                0);
        }

        UiMgr.Instance.EnableValidateBtn = true;
    }

    public void ValidateTile(bool sendToLogic = true)
    {
        Material[] mats = _currentPreviews[0].GetComponent<MeshRenderer>().materials;
        foreach (Material mat in mats.Where((_, i) => i != 1))
        {
            mat.SetRenderMode(MaterialExtensions.BlendMode.Opaque);
            mat.color = mat.color.With(a: 1);
        }

        Rotation rot = RotationExt.Of(Mathf.Round(_currentPreviews[0].transform.rotation.eulerAngles.y));

        _currentPreviews = null;
        if (sendToLogic)
            GameMgr.Instance.Phase1(new(_gos[_currentFf].point, rot), rot);
    }

    public void ReputTile(Vector2Int pos, Rotation rot)
    {
        _gos = null;
        Vector3 p = new(pos.x, 0, pos.y);
        if (!GameMgr.Instance.IsVoid(pos))
            p.y = (GameMgr.Instance.LevelAt(pos) - 1) * yOffset;
        p.Scale(new(xOffset, 1, zOffset));

        if (pos.x % 2 != 0)
            p.z += zOffset / 2;

        _currentFf = new();
        _gos = new()
        {
            [_currentFf] = new(pos, rot)
        };
        PutTile(p);
        ValidateTile(false);
        Destroy(_currentFf);
        _currentFf = null;
    }

    public void PutAiTile(Vector2Int pos, Vector3 p, Rotation rot, Chunk chunk)
    {
        UiMgr.Instance.Phase1();
        
        _gos = null;

        _currentFf = new();
        _gos = new()
        {
            [_currentFf] = new(pos, rot)
        };
        
        UiMgr.Instance.ChangeTileColor(chunk);
        PutTile(p);
        ValidateTile(false);
        Destroy(_currentFf);
        _currentFf = null;
    }

    private void PutBuild(Vector3 _) => PutBuild(GameMgr.Instance.actualPlayer.ID.GetColor());

    private void PutBuild(Color color)
    {
        Vector2Int currentPos = _gos[_currentFf].point;
        List<Vector2Int> tmp = new() { currentPos };
        if (_currentBuild == Building.Barrack)
            tmp = GameMgr.Instance.FindBiomesAroundVillage(currentPos);

        if (_currentPreviews is null || _currentPreviews.Length < tmp.Count)
            _currentPreviews = new GameObject[tmp.Count];

        for (int i = 0; i < tmp.Count; i++)
        {
            if (_currentPreviews[i] == null)
            {
                _currentPreviews[i] = Instantiate(_currentBuild switch
                {
                    Building.Barrack => barrack,
                    Building.Tower => tower,
                    Building.Temple => temple
                }, boardParent);

                _currentPreviews[i].transform.localScale = _buildsScale[_currentBuild];
                Material[] mats = _currentPreviews[i].GetComponent<MeshRenderer>().materials;
                foreach (Material mat in mats)
                {
                    mat.SetRenderMode(MaterialExtensions.BlendMode.Transparent);
                    mat.color = color.With(a: .75f);
                }
            }

            _currentPreviews[i].transform.position = _gos
                .First(go => go.Value.point == tmp[i])
                .Key.transform.position;
        }

        for (int i = tmp.Count; i < _currentPreviews.Length; i++)
            Destroy(_currentPreviews[i]);

        UiMgr.Instance.EnableValidateBtn = true;
    }

    public void ValidateBuild(bool sendToLogic = true)
    {
        foreach (GameObject currentPreview in _currentPreviews)
        {
            Material[] mats = currentPreview.GetComponent<MeshRenderer>().materials;
            foreach (Material mat in mats.Where((_, i) => i != 1))
            {
                mat.SetRenderMode(MaterialExtensions.BlendMode.Opaque);
                mat.color = mat.color.With(a: 1);
            }
        }

        _currentPreviews = null;
        if (sendToLogic)
            GameMgr.Instance.Phase2(_gos[_currentFf].point, _currentBuild);
    }

    private static Vector3 V2IToV3(Vector2Int v)
    {
        Vector3 v3 = new(v.x, 0, v.y);
        if (!GameMgr.Instance.IsVoid(v))
            v3.y = GameMgr.Instance.LevelAt(v) * yOffset;
        v3.Scale(new(xOffset, 1, zOffset));

        if (v.x % 2 != 0)
            v3.z += zOffset / 2;

        return v3;
    }

    public void ReputBuild(Vector2Int pos, Building b, Player player)
    {
        _currentBuild = b;
        _gos = null;

        _gos = GameMgr.Instance.FindBiomesAroundVillage(pos).ToDictionary
        (
            static p => new GameObject
            {
                transform =
                {
                    position = V2IToV3(p)
                }
            },
            static p => new PointRotation(p)
        );

        _currentFf = _gos.Keys.First();

        PutBuild(player.ID.GetColor());
        ValidateBuild(false);
        Destroy(_currentFf);
        _currentFf = null;
    }

    private void RotateTile()
    {
        Rotation rot;

        do
        {
            _currentPreviews[0].transform.Rotate(new(0, 360f / 6, 0), Space.World);
            rot = RotationExt.Of(Mathf.Round(_currentPreviews[0].transform.rotation.eulerAngles.y));
        } while (_gos?[_currentFf]?.rotations?[(int) rot] == false);
    }

    public void SetFeedForwards1()
    {
        ClearFeedForward();
        if (!(GameMgr.Instance.actualPlayer is AI))
        {
            foreach (PointRotation pr in GameMgr.Instance.ChunkSlots())
            {
                Vector3 pos = new(pr.point.x, 0, pr.point.y);
                if (!GameMgr.Instance.IsVoid(pr.point))
                    pos.y = GameMgr.Instance.LevelAt(pr.point) * yOffset;
                pos.Scale(new(xOffset, 1, zOffset));
                if (pr.point.x % 2 != 0)
                    pos.z += zOffset / 2;
                _gos[SetFeedForward(pos)] = pr;
            }
        }
    }

    public void SetFeedForwards2(Building build)
    {
        if (_currentPreviews is not null)
        {
            foreach (GameObject go in _currentPreviews)
                Destroy(go);
            _currentPreviews = null;
        }

        ClearFeedForward();
        _currentBuild = build;
        Vector2Int[] poss = build switch
        {
            Building.Barrack => GameMgr.Instance.BarracksSlots(),
            Building.Tower => GameMgr.Instance.TowerSlots(GameMgr.Instance.actualPlayer),
            Building.Temple => GameMgr.Instance.TempleSlots(GameMgr.Instance.actualPlayer)
        };

        foreach (Vector2Int p in poss)
        {
            Vector3 pos = new(p.x, 0, p.y);
            if (!GameMgr.Instance.IsVoid(p))
                pos.y = GameMgr.Instance.LevelAt(p) * yOffset;
            pos.Scale(new(xOffset, 1, zOffset));
            if (p.x % 2 != 0)
                pos.z += zOffset / 2;
            _gos[SetFeedForward(pos)] = new(p);
        }
    }

    public GameObject SetFeedForward(Vector3 pos)
    {
        GameObject go = Instantiate(feedForward, pos, Quaternion.Euler(-90, -90, 0), feedForwardParent);
        go.GetComponent<MeshRenderer>().materials[1].color = GameMgr.Instance.actualPlayer.ID.GetColor();
        return go;
    }

    public void ClearFeedForward()
    {
        UiMgr.Instance.EnableValidateBtn = false;
        _gos = new();
        foreach (Transform t in feedForwardParent)
            Destroy(t.gameObject);
    }

    public void RemoveTile(Vector2Int pos)
    {
        Vector3 p = new(pos.x, 0, pos.y);
        if (!GameMgr.Instance.IsVoid(pos))
            p.y = GameMgr.Instance.LevelAt(pos) * yOffset;
        p.Scale(new(xOffset, 1, zOffset));
        if (pos.x % 2 != 0)
            p.z += zOffset / 2;

        Destroy(boardParent.transform.Cast<Transform>().First(t => t.position == p).gameObject);
    }

    public void RemoveBuild(Vector2Int[] poss)
    {
        foreach (Vector2Int pos in poss)
        {
            Vector3 p = new(pos.x, 0, pos.y);
            if (!GameMgr.Instance.IsVoid(pos))
                p.y = GameMgr.Instance.LevelAt(pos) * yOffset;
            p.Scale(new(xOffset, 1, zOffset));

            if (pos.x % 2 != 0)
                p.z += zOffset / 2;

            Destroy(boardParent.transform.Cast<Transform>().First(t => t.position == p).gameObject);
        }
    }
}