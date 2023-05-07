using System;

using Taluva.Model;

using UnityEngine;
using UnityEngine.UI;

using Utils;

using GameObject = UnityEngine.GameObject;
using Outline = Imports.QuickOutline.Scripts.Outline;
using Random = UnityEngine.Random;

namespace UI
{
    public class UiMgr : MonoBehaviour
    {
        [SerializeField]
        private Text uiNbTiles;

        private const string NB_TILES_PLACEHOLDER = "%nb% tuiles restantes";

        [SerializeField]
        private int nbTilesPerPlayers = 12;

        private int _nbTiles;

        private int NbTiles
        {
            get => _nbTiles;
            set
            {
                _nbTiles = value;
                uiNbTiles.text = NB_TILES_PLACEHOLDER.Replace("%nb%", value.ToString());
            }
        }

        [SerializeField]
        private GameObject playerPrefab;

        private readonly GameObject[] _guis = new GameObject[4];

        [SerializeField]
        private Text[] currentPlayerBuild;

        [SerializeField]
        private GameObject tile, builds;

        private float _defaultBuildsY;

        [SerializeField]
        private GameObject menuCanva;

        [Header("Debug")]
        [SerializeField]
        private KeyCode phase1 = KeyCode.Keypad1;

        [SerializeField]
        private KeyCode phase2 = KeyCode.Keypad2,
            nextPlayer = KeyCode.KeypadEnter,
            nextPhase = KeyCode.Return,
            menu = KeyCode.Escape;

        [SerializeField]
        private GameObject hexTile, boardParent;

        [SerializeField]
        private Vector3[] hexTilePositions;

        private sbyte _phase;
        private sbyte Phase
        {
            get => _phase;
            set
            {
                switch (value)
                {
                    case > 2:
                        NextPlayer();
                        _phase = 1;
                        break;
                    case <= 0:
                        PreviousPlayer();
                        _phase = 2;
                        break;
                    default:
                        _phase = value;
                        break;
                }

                (_phase switch
                {
                    1 => Phase1,
                    2 => Phase2,
                    _ => (Action) null
                })?.Invoke();
            }
        }

        #region Unity events

        private void Start()
        {
            Phase = 1;
            NbTiles = nbTilesPerPlayers * PlayerMgr.Instance.Length;
            _defaultBuildsY = builds.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition.y;
        }

        private void Update()
        {
            if (Input.GetKeyDown(nextPlayer))
                NextPlayer();

            if (Input.GetKeyDown(nextPhase))
                Next();

            if (Input.GetKeyDown(phase1))
                Phase = 1;

            if (Input.GetKeyDown(phase2))
                Phase = 2;

            if (Input.GetKeyDown(menu))
                ToggleMenu();
        }

        // Start is called before the first frame update
        private void OnGUI()
        {
            for (int i = 0; i < PlayerMgr.Instance.Length; i++)
            {
                _guis[i] ??= Instantiate(playerPrefab, transform);

                _guis[i].GetComponent<Image>().color =
                    i == PlayerMgr.Instance.CurrentIndex ? Color.white : new(.75f, .75f, .75f);

                _guis[i].transform.GetChild(0).GetComponent<Text>().text = PlayerMgr.Instance[i].Name;
                _guis[i].transform.GetChild(1).GetComponent<Image>().color = PlayerMgr.Instance[i].Color;

                _guis[i].transform.GetChild(2).GetComponentInChildren<Text>().text =
                    PlayerMgr.Instance[i].Builds[0].ToString();
                _guis[i].transform.GetChild(3).GetComponentInChildren<Text>().text =
                    PlayerMgr.Instance[i].Builds[1].ToString();
                _guis[i].transform.GetChild(4).GetComponentInChildren<Text>().text =
                    PlayerMgr.Instance[i].Builds[2].ToString();

                RectTransform rt = _guis[i].GetComponent<RectTransform>();

                rt.pivot = Vector2.one;
                rt.anchorMin = Vector2.one;
                rt.anchorMax = Vector2.one;
                rt.anchoredPosition = new(-10, -10 - 110 * i);
            }

            currentPlayerBuild[0].text = PlayerMgr.Instance.Current.Builds[0].ToString();
            currentPlayerBuild[1].text = PlayerMgr.Instance.Current.Builds[1].ToString();
            currentPlayerBuild[2].text = PlayerMgr.Instance.Current.Builds[2].ToString();
        }

        #endregion

        public void Next() => Phase++;

        private void Phase1()
        {
            BiomeColor[] values = (BiomeColor[]) Enum.GetValues(typeof(BiomeColor));
            MeshRenderer mr = tile.transform.GetComponentInChildren<MeshRenderer>();

            mr.materials[3].color = values[Random.Range(0, values.Length - 1)].GetColor();
            mr.materials[2].color = values[Random.Range(0, values.Length - 1)].GetColor();
            mr.materials[0].color = values[Random.Range(0, values.Length - 1)].GetColor();
            tile.SetActive(true);
            builds.SetActive(false);
        }

        private void Phase2()
        {
            tile.SetActive(false);
            builds.SetActive(true);
            NbTiles--;
            UpBuild(0);

            ref Vector3 pos = ref hexTilePositions[Random.Range(0, hexTilePositions.Length)];
            Instantiate(hexTile, pos, Quaternion.Euler(-90, 0, -90), boardParent.transform);
            pos.y += 0.31f;
        }

        public void Undo() => Phase--;
        public void Redo() => Phase++;

        private void NextPlayer() => PlayerMgr.Instance.CurrentIndex++;
        private void PreviousPlayer() => PlayerMgr.Instance.CurrentIndex--;

        public void ToggleMenu()
        {
            menuCanva.SetActive(!menuCanva.activeSelf);
        }

        public void UpBuild(int i)
        {
            RectTransform child;
            foreach (Transform t in builds.transform)
            {
                child = t.GetComponent<RectTransform>();
                child.anchoredPosition = child.anchoredPosition.With(y: _defaultBuildsY);
                t.GetComponentInChildren<Outline>().enabled = false;
            }

            child = builds.transform.GetChild(i).GetComponent<RectTransform>();
            child.anchoredPosition = child.anchoredPosition.With(y: 20);
            Outline outline = child.GetComponentInChildren<Outline>();
            outline.enabled = true;
            outline.OutlineColor = PlayerMgr.Instance.Current.Color;
        }
    }
}