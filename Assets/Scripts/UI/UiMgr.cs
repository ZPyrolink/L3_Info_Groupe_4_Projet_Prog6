using System;

using Taluva.Model;

using UnityEngine;
using UnityEngine.UI;

using Utils;

using Wrapper;

namespace UI
{
    public class UiMgr : MonoBehaviour
    {
        public static UiMgr Instance { get; private set; }

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

        public GameObject CurrentTile => tile.transform.GetChild(0).gameObject;

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

        private sbyte _phase;
        public sbyte Phase
        {
            get => _phase;
            private set
            {
                switch (value)
                {
                    case > 2:
                        // NextPlayer();
                        _phase = 1;
                        break;
                    case <= 0:
                        // PreviousPlayer();
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
            NbTiles = nbTilesPerPlayers * GameMgr.Instance.NbPlayers;
            Phase = 1;
            _defaultBuildsY = builds.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition.y;
            Instance = this;
        }

        private void Update()
        {
            // if (Input.GetKeyDown(nextPlayer))
            //     NextPlayer();

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
            for (int i = 0; i < GameMgr.Instance.NbPlayers; i++)
            {
                if (_guis[i] is null)
                {
                    _guis[i] = Instantiate(playerPrefab, transform);
                    foreach (MeshRenderer mr in _guis[i].GetComponentsInChildren<MeshRenderer>())
                        mr.material.color = GameMgr.Instance.players[i].ID.GetColor();
                }

                _guis[i].GetComponent<Image>().color =
                    i == GameMgr.Instance.ActualPlayerIndex ? Color.white : new(.75f, .75f, .75f);

                foreach (Animator anim in _guis[i].GetComponentsInChildren<Animator>())
                    if (GameMgr.Instance.ActualPlayerIndex == i)
                    {
                        anim.enabled = true;
                    }
                    else
                    {
                        anim.enabled = false;
                        anim.transform.localRotation = Quaternion.Euler(-90, 0, 0);
                    }


                _guis[i].transform.GetChild(0).GetComponent<Text>().text = $"Player {i}";
                _guis[i].transform.GetChild(1).GetComponent<Image>().color = GameMgr.Instance.players[i].ID.GetColor();

                _guis[i].transform.GetChild(2).GetComponentInChildren<Text>().text =
                    GameMgr.Instance.players[i].nbBarrack.ToString();
                _guis[i].transform.GetChild(3).GetComponentInChildren<Text>().text =
                    GameMgr.Instance.players[i].nbTowers.ToString();
                _guis[i].transform.GetChild(4).GetComponentInChildren<Text>().text =
                    GameMgr.Instance.players[i].nbTemple.ToString();

                RectTransform rt = _guis[i].GetComponent<RectTransform>();

                rt.pivot = Vector2.one;
                rt.anchorMin = Vector2.one;
                rt.anchorMax = Vector2.one;
                rt.anchoredPosition = new(-10, -10 - 110 * i);
            }

            currentPlayerBuild[0].text = GameMgr.Instance.actualPlayer.nbBarrack.ToString();
            currentPlayerBuild[1].text = GameMgr.Instance.actualPlayer.nbTowers.ToString();
            currentPlayerBuild[2].text = GameMgr.Instance.actualPlayer.nbTemple.ToString();
        }

        #endregion

        public void Next()
        {
            (Phase switch
            {
                1 => (Action) TilesMgr.Instance.ValidateTile,
                2 => TilesMgr.Instance.ValidateBuild
            }).Invoke();

            Phase++;
        }

        private void Phase1()
        {
            if (NbTiles == nbTilesPerPlayers * GameMgr.Instance.NbPlayers)
                TilesMgr.Instance.SetFeedForward(Vector3.zero);
            else
                TilesMgr.Instance.SetFeedForwards1();

            CurrentTile.SetActive(true);

            MeshRenderer mr = tile.transform.GetComponentInChildren<MeshRenderer>();
            mr.materials[0].color = GameMgr.Instance.actualChunk.Coords[1].ActualBiome.GetColor();
            mr.materials[2].color = Biomes.Volcano.GetColor();
            mr.materials[3].color = GameMgr.Instance.actualChunk.Coords[2].ActualBiome.GetColor();

            tile.SetActive(true);
            builds.SetActive(false);
        }

        private void Phase2()
        {
            tile.SetActive(false);
            builds.SetActive(true);

            foreach (MeshRenderer mr in builds.GetComponentsInChildren<MeshRenderer>())
                mr.material.color = GameMgr.Instance.actualPlayer.ID.GetColor();

            NbTiles--;
            UpBuild(0);
        }

        public void Undo() => Phase--;
        public void Redo() => Phase++;

        public void ToggleMenu()
        {
            menuCanva.SetActive(!menuCanva.activeSelf);
        }

        public void UpBuild(int i)
        {
            RectTransform child;
            Animator anim;
            
            foreach (Transform t in builds.transform)
            {
                child = t.GetComponent<RectTransform>();
                child.anchoredPosition = child.anchoredPosition.With(y: _defaultBuildsY);

                anim = t.GetComponentInChildren<Animator>();
                anim.enabled = false;
                anim.transform.localRotation = Quaternion.Euler(-90, -90, 0);
            }

            TilesMgr.Instance.SetFeedForwards2((Building) i + 1);

            child = builds.transform.GetChild(i).GetComponent<RectTransform>();
            child.anchoredPosition = child.anchoredPosition.With(y: 20);
            anim = child.GetComponentInChildren<Animator>();
            anim.enabled = true;
        }
    }
}