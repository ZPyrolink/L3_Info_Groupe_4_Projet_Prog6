using System;

using Taluva.Model;

using UnityEngine;
using UnityEngine.Serialization;
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

        private int NbTiles
        {
            get => GameMgr.Instance.pile.NbKeeping;
            set => uiNbTiles.text = NB_TILES_PLACEHOLDER.Replace("%nb%", value.ToString());
        }

        [SerializeField]
        private GameObject playerPrefab;

        private readonly GameObject[] _guis = new GameObject[4];

        [FormerlySerializedAs("currentPlayerBuild")]
        [SerializeField]
        private Text[] currentPlayerBuildCount;

        [SerializeField]
        private GameObject currentTile,
            builds;

        public GameObject CurrentTile => currentTile.transform.GetChild(0).gameObject;

        private float _defaultBuildsY;

        [SerializeField]
        private GameObject menuCanva;

        [SerializeField]
        private KeyCode nextPhase = KeyCode.Return, menu = KeyCode.Escape;

        #region Unity events

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            NbTiles = NbTiles;
            _defaultBuildsY = builds.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition.y;
        }

        private void Update()
        {
            if (Input.GetKeyDown(nextPhase))
                Next();

            if (Input.GetKeyDown(menu))
                ToggleMenu();
        }

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

            currentPlayerBuildCount[0].text = GameMgr.Instance.actualPlayer.nbBarrack.ToString();
            currentPlayerBuildCount[1].text = GameMgr.Instance.actualPlayer.nbTowers.ToString();
            currentPlayerBuildCount[2].text = GameMgr.Instance.actualPlayer.nbTemple.ToString();
        }

        #endregion

        public void Next()
        {
            (GameMgr.Instance.actualPhase switch
            {
                TurnPhase.SelectCells => (Action) TilesMgr.Instance.ValidateTile,
                TurnPhase.PlaceBuilding => TilesMgr.Instance.ValidateBuild
            }).Invoke();
        }

        public void Phase1()
        {
            if (NbTiles == ListeChunk.Count)
                TilesMgr.Instance.SetFeedForward(Vector3.zero);
            else
                TilesMgr.Instance.SetFeedForwards1();

            CurrentTile.SetActive(true);

            MeshRenderer mr = currentTile.transform.GetComponentInChildren<MeshRenderer>();
            mr.materials[0].color = GameMgr.Instance.actualChunk.Coords[1].ActualBiome.GetColor();
            mr.materials[2].color = Biomes.Volcano.GetColor();
            mr.materials[3].color = GameMgr.Instance.actualChunk.Coords[2].ActualBiome.GetColor();

            currentTile.SetActive(true);
            builds.SetActive(false);
        }

        public void Phase2()
        {
            currentTile.SetActive(false);
            builds.SetActive(true);

            foreach (MeshRenderer mr in builds.GetComponentsInChildren<MeshRenderer>())
                mr.material.color = GameMgr.Instance.actualPlayer.ID.GetColor();

            NbTiles--;
            UpBuild(0);
        }

        public void Undo() => GameMgr.Instance.Undo();
        public void Redo() => GameMgr.Instance.Redo();

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