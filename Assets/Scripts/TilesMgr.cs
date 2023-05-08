using System.Linq;

using Imports.QuickOutline.Scripts;

using Taluva.Model;

using UI;

using UnityEngine;

using Utils;

public class TilesMgr : MonoBehaviour
{
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
        foreach (Material mat in _current.GetComponent<MeshRenderer>().materials)
            mat.color = mat.color.With(a: 1);

        Cell left = new(Biomes.Desert), right = new(Biomes.Desert);
        
        Chunk c = new(1, left, right);
        _board.AddChunk(c, new(PlayerColor.Blue), new(Vector2Int.zero, Rotation.N));

        _current = null;
        ClearFeedForward();
    }

    private void RotateTile() => _current.transform.Rotate(new(0, 360f / 6, 0), Space.World);

    public void SetFeedForward()
    {
        foreach (PointRotation pr in _board.GetChunkSlots())
            Debug.Log(pr);
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