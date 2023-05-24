using System;
using System.Collections.Generic;
using System.Linq;

using Taluva.Model;
using Taluva.Model.AI;

using UI;

using UnityEngine;

using UnityUtils;

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

    private readonly Dictionary<Building, Vector3> _buildsScale = new()
    {
        [Building.Barrack] = new(100, 100, 100),
        [Building.Tower] = new(70, 70, 100),
        [Building.Temple] = new(100, 100, 100)
    };

    public static Dictionary<Building, int> BuildOwnerMatIndex { get; } = new()
    {
        [Building.Barrack] = 1,
        [Building.Tower] = 1,
        [Building.Temple] = 4
    };

    private Dictionary<GameObject, PointRotation> _gos;

    [SerializeField]
    private List<KeyValueS<Biomes, Material>> materials;

    public List<KeyValueS<Biomes, Material>> Materials => materials;

    [SerializeField]
    private List<KeyValueS<Biomes, Material>> transparentMaterials;

    [SerializeField]
    private EditableDictionary<Biomes, GameObject> biomeProps;

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
                if (_currentFf != null && GameMgr.Instance.CurrentPhase == TurnPhase.SelectCells)
                    _currentFf.GetComponent<MeshRenderer>().materials[0].SetInt(Shader.PropertyToID("_Rotation"), 0);
                
                _currentFf = hit.transform.gameObject;
                switch (GameMgr.Instance.CurrentPhase)
                {
                    case TurnPhase.SelectCells:
                        _currentFf.GetComponent<MeshRenderer>().materials[0].SetInt(Shader.PropertyToID("_Rotation"), 1);
                        PutTile(hit.transform.position);
                        break;
                    case TurnPhase.PlaceBuilding:
                        PutBuild(GameMgr.Instance.CurrentPlayer.IdColor);
                        break;
                }
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

            ChangeTileColor(GameMgr.Instance.CurrentChunk, _currentPreviews[0].GetComponent<MeshRenderer>(),
                transparentMaterials);
        }

        if (_currentPreviews[0].transform.position == pos)
            RotateTile();
        else
        {
            _currentPreviews[0].transform.position = pos;
            _currentPreviews[0].transform.rotation = Quaternion.Euler(270,
                _gos == null ? 270 : ((Rotation) Array.IndexOf(_gos[_currentFf].Rotations, true)).YDegree(),
                0);
        }

        UiMgr.Instance.InteractiveValidate = GameMgr.Instance.CurrentPhase != TurnPhase.IAPlays;
    }

    public void PreviewTile(PointRotation pos)
    {
        Rotation r = Rotation.N;
        for (int i = 0; i < pos.Rotations.Length; i++)
        {
            if (pos.Rotations[i])
                r = (Rotation) i;
        }

        PutAiTile(pos.Point, V2IToV3(pos.Point), r, GameMgr.Instance.CurrentChunk, true);
    }

    public void PreviewBuild(Vector2Int pos, Building b)
    {
        PutAiBuild(GameMgr.Instance.FindBiomesAroundVillage(pos).ToArray(), b, GameMgr.Instance.CurrentPlayer, true);
    }

    public void ValidateTile(bool sendToLogic = true)
    {
        ChangeTileColor(GameMgr.Instance.CurrentChunk, _currentPreviews[0].GetComponent<MeshRenderer>(), materials);

        Rotation rot = RotationExt.Of(Mathf.Round(_currentPreviews[0].transform.rotation.eulerAngles.y));
        
        Vector2Int volcanoPos = _gos[_currentFf].Point;

        if (!GameMgr.Instance.IsVoid(volcanoPos))
            ClearInformations(volcanoPos, rot);
        
        _currentPreviews = null;

        if (sendToLogic)
            GameMgr.Instance.Phase1(new(_gos[_currentFf].Point, rot), rot);
        
        foreach (Cell c in GameMgr.Instance.gameBoard.WorldMap[volcanoPos].ParentChunk.Coords)
        {
            if (!biomeProps.ContainsKey(c.CurrentBiome))
                continue;
            
            Vector2Int coord = GameMgr.Instance.gameBoard.GetCellCoord(c);
            Vector3 propsPos = V2IToV3(coord);
            Vector3 propsRot = V2IToEul(coord);
            
            Instantiate(biomeProps[c.CurrentBiome], propsPos, Quaternion.Euler(propsRot), boardParent);
        }
    }

    public static void ChangeTileColor(Chunk chunk, MeshRenderer mr, List<KeyValueS<Biomes, Material>> mats)
    {
        Material[] mrs = mr.materials;
        Cell[] coords = chunk.Coords;

        void SetMat(int mrIndex, int coordIndex)
        {
            Material tmp = mats
                .FirstOrDefault(kv => kv.Key == coords[coordIndex].CurrentBiome)?.Value;

            if (tmp is null) // ToDo: Remove when all materials are ready
                mrs[mrIndex].color = coords[coordIndex].CurrentBiome.GetColor();
            else
                mrs[mrIndex] = tmp;
        }

        SetMat(0, 1);
        SetMat(3, 2);
        mrs[2] = mats.First(kv => kv.Key == Biomes.Volcano).Value;

        mr.materials = mrs;
    }

    public void ClearInformations(Vector2Int chunkPos, Rotation rotation)
    {
        if (!GameMgr.Instance.gameBoard.WorldMap.IsVoid(chunkPos))
        {
            Vector2Int[] positionChunk = GameMgr.Instance.gameBoard.GetChunksCoords(chunkPos, rotation);
            List<Vector2Int[]> chunkRecouvert = new();

            foreach (Vector2Int positionCell in positionChunk)
            {
                Chunk tile = GameMgr.Instance.gameBoard.WorldMap[positionCell].ParentChunk;
                Vector2Int posVolcano = tile.Coords[0].position;
                Vector2Int[] positions = GameMgr.Instance.gameBoard.GetChunksCoords(posVolcano, tile.Rotation);
                if (!chunkRecouvert.Contains(positions))
                    chunkRecouvert.Add(positions);
            }

            foreach (Vector2Int[] positions in chunkRecouvert)
            {
                for (int i = 0; i < positions.Length; i++)
                {
                    for (int j = 0; j < positionChunk.Length; j++)
                    {
                        if (positions[i] == positionChunk[j])
                        {
                            ClearHouseAndBiomes(positions, i);
                        }
                    }
                }
            }
        }
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

    public void PutAiTile(Vector2Int pos, Vector3 p, Rotation rot, Chunk chunk, bool preview = false)
    {
        UiMgr.Instance.Phase1();

        if (!preview)
        {
            _gos = null;

            _currentFf = new();
            _gos = new()
            {
                [_currentFf] = new(pos, rot)
            };
        }
        else
        {
            for (int i = 0; i < feedForwardParent.childCount; ++i)
            {
                if (feedForwardParent.GetChild(i).transform.position == p)
                    _currentFf = feedForwardParent.GetChild(i).gameObject;
            }
        }

        UiMgr.Instance.ChangeTileColor(chunk);
        PutTile(p);
        if (!preview)
        {
            ValidateTile(false);
            Destroy(_currentFf);
            _currentFf = null;
        }
    }

    public void ClearCurrentPreviews()
    {
        foreach (GameObject currentPreview in _currentPreviews)
            Destroy(currentPreview);
        _currentPreviews = null;
    }

    public bool CurrentPreviewsNotNull => _currentPreviews != null;

    [Obsolete("Use PutBuild(Color) instead", true)]
    private void PutBuild(Vector3 _) => PutBuild(GameMgr.Instance.CurrentPlayer.IdColor);

    private void PutBuild(Color color, Vector2Int? position = null)
    {
        Vector2Int currentPos;
        if (position != null)
            currentPos = (Vector2Int) position;
        else
            currentPos = _gos[_currentFf].Point;

        List<Vector2Int> tmp = new() { currentPos };
        if (_currentBuild == Building.Barrack)
            tmp = GameMgr.Instance.FindBiomesAroundVillage(currentPos);

        if (_currentPreviews is not null)
            foreach (GameObject currentPreview in _currentPreviews)
                Destroy(currentPreview);

        _currentPreviews = null;

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
                MeshRenderer mr = _currentPreviews[i].GetComponent<MeshRenderer>();
                Material[] mats = mr.materials;

                if (_currentBuild == Building.Barrack)
                    foreach (Material mat in mats)
                        mat.SetFloat(Shader.PropertyToID("_Level"), GameMgr.Instance.LevelAt(tmp[i]));

                foreach (Material mat in mats)
                    mat.SetInt(Shader.PropertyToID("_Preview"), 1);

                mats[BuildOwnerMatIndex[_currentBuild]].color = color;

                mr.materials = mats;
            }

            _currentPreviews[i].transform.position = _gos
                .First(go => go.Value.Point == tmp[i])
                .Key.transform.position;

            _currentPreviews[i].transform.rotation = Quaternion.Euler(V2IToEul(tmp[i]));
        }

        for (int i = tmp.Count; i < _currentPreviews.Length; i++)
            Destroy(_currentPreviews[i]);

        UiMgr.Instance.InteractiveValidate = GameMgr.Instance.CurrentPhase != TurnPhase.IAPlays;
    }

    public void ValidateBuild(bool sendToLogic = true)
    {
        foreach (GameObject currentPreview in _currentPreviews)
        {
            MeshRenderer mr = currentPreview.GetComponent<MeshRenderer>();
            Material[] mrs = mr.materials;
            foreach (Material mat in mrs)
                mat.SetInt(Shader.PropertyToID("_Preview"), 0);
            mr.materials = mrs;
        }

        _currentPreviews = null;
        if (sendToLogic)
            GameMgr.Instance.Phase2(_gos[_currentFf].Point, _currentBuild);
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

    private static Vector3 V2IToEul(Vector2Int v) => new(270,
        GameMgr.Instance.IsVoid(v) ? 270 : GameMgr.Instance.gameBoard.WorldMap[v].ParentChunk.Rotation.YDegree(),
        GameMgr.Instance.CellPositionInChunk(v) switch { 1 => -120, 2 => 120, _ => 0 });

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

    public void PutAiBuild(Vector2Int[] pos, Building b, Player player, bool preview = false)
    {
        _currentBuild = b;

        if (!preview)
        {
            _gos = null;

            _gos = pos.ToDictionary
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
        }
        else
        {
            for (int i = 0; i < feedForwardParent.childCount; ++i)
            {
                if (feedForwardParent.GetChild(i).transform.position == V2IToV3(pos[0]))
                    _currentFf = feedForwardParent.GetChild(i).gameObject;
            }
        }


        if (_currentPreviews is null || _currentPreviews.Length < pos.Length)
            _currentPreviews = new GameObject[pos.Length];

        for (int i = 0; i < pos.Length; i++)
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
                    mat.color = player.ID.GetColor().With(a: .75f);
                }
                if (_currentBuild == Building.Barrack)
                    foreach (Material mat in mats)
                        mat.SetFloat(Shader.PropertyToID("_Level"), GameMgr.Instance.LevelAt(pos[i]));
            }

            _currentPreviews[i].transform.position = _gos
                .First(go => go.Value.Point == pos[i])
                .Key.transform.position;
        }

        for (int i = pos.Length; i < _currentPreviews.Length; i++)
            Destroy(_currentPreviews[i]);

        if (!preview)
        {
            ValidateBuild(false);
            Destroy(_currentFf);
            _currentFf = null;
        }
    }

    private void RotateTile()
    {
        Rotation rot;

        do
        {
            _currentPreviews[0].transform.Rotate(new(0, 360f / 6, 0), Space.World);
            rot = RotationExt.Of(Mathf.Round(_currentPreviews[0].transform.rotation.eulerAngles.y));
        } while (_gos?[_currentFf]?.Rotations?[(int) rot] == false);
    }

    public Transform FindObject(Vector2Int pos)
    {
        Vector3 v3 = V2IToV3(pos);

        return boardParent.Cast<Transform>().FirstOrDefault(t => t.position == v3);
    }

    private void ClearHouseAndBiomes(Vector2Int[] posChunk, int cell)
    {
        Transform c = FindObject(posChunk[cell]);
        Destroy(c.gameObject);
        if (cell != 0)
        {
            c = FindObject(posChunk[cell]);
            if (c != null)
                Destroy(c.gameObject);
        }
    }

    public void SetFeedForwards1()
    {
        ClearFeedForward();
        if (GameMgr.Instance.CurrentPlayer is AI)
            return;

        foreach (PointRotation pr in GameMgr.Instance.ChunkSlots())
            _gos[SetFeedForward(V2IToV3(pr.Point), V2IToEul(pr.Point))] = pr;
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
            Building.Tower => GameMgr.Instance.TowerSlots(GameMgr.Instance.CurrentPlayer),
            Building.Temple => GameMgr.Instance.TempleSlots(GameMgr.Instance.CurrentPlayer)
        };

        foreach (Vector2Int p in poss)
            _gos[SetFeedForward(V2IToV3(p), V2IToEul(p))] = new(p);
    }

    public GameObject SetFeedForward(Vector3 pos, Vector3 euler)
    {
        GameObject go = Instantiate(feedForward, pos, Quaternion.Euler(euler), feedForwardParent);
        go.GetComponent<MeshRenderer>().materials[1].color = GameMgr.Instance.CurrentPlayer.IdColor;
        return go;
    }

    public void ClearFeedForward()
    {
        UiMgr.Instance.InteractiveValidate = false;
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