using System.Linq;

using Imports.QuickOutline.Scripts;

using Taluva.Model;
using Taluva.Utils;

using UI;

using UnityEngine;

using Utils;

public class TilesMgr : MonoBehaviour
{
    private const float xOffset = 1.5f, yOffset = .246f, zOffset = 1.73205f;
    public static TilesMgr Instance { get; private set; }

    private Board _board;

    [SerializeField]
    private Transform boardParent;

    private GameObject _current;

    [SerializeField]
    private GameObject feedForward;

    [SerializeField]
    private Transform feedForwardParent;

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
            _current.transform.rotation = Quaternion.Euler(-90, 0, -90);
            _current.layer = LayerMask.NameToLayer("Default");
            foreach (Material mat in _current.GetComponent<MeshRenderer>().materials)
                mat.color = mat.color.With(a: .5f);
        }

        if (_current.transform.position == pos)
            RotateTile();
        else
            _current.transform.position = pos;
    }

    public void ValidateTile()
    {
        Material[] mats = _current.GetComponent<MeshRenderer>().materials;
        foreach (Material mat in mats)
            mat.color = mat.color.With(a: 1);

        Cell left = new(BiomeColorExt.Of(mats[0].color)), right = new(BiomeColorExt.Of(mats[3].color));

        (Vector2Int pos, Rotation rot, int level) = GetPr();
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

        return (pos, rot, (int) (_current.transform.position.y / yOffset));
    }

    private void RotateTile() => _current.transform.Rotate(new(0, 360f / 6, 0), Space.World);

    public void SetFeedForward()
    {
        foreach (PointRotation pr in _board.GetChunkSlots())
        {
            Vector3 pos = new(pr.point.x, 0, pr.point.y);
            if (!_board.WorldMap.IsVoid(pr.point))
                pos.y = (_board.WorldMap[pr.point].ParentCunk.Level + 1) * yOffset;
            pos.Scale(new(xOffset, 1, zOffset));
            if (pr.point.x % 2 != 0)
                pos.z += zOffset / 2;
            SetFeedForward(pos);
        }
    }

    public void SetFeedForward(Vector3 pos) => Instantiate(feedForward, pos, Quaternion.identity, feedForwardParent);

    public void ClearFeedForward()
    {
        foreach (Transform t in feedForwardParent)
            Destroy(t.gameObject);
    }

    public void ClearOutline()
    {
        foreach (Outline o in GameObject.FindGameObjectsWithTag("Tile").Select(go => go.GetComponent<Outline>()))
            o.enabled = false;
    }
}