using System;

using Taluva.Controller;
using Taluva.Model;
using Taluva.Model.AI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

using Utils;

using Wrapper;

namespace UI
{
    public class UiMgr : MonoBehaviourMgr<UiMgr>
    {
        [SerializeField]
        private Text uiNbTiles;

        private const string NB_TILES_PLACEHOLDER = "%nb% tuiles restantes";

        private int NbTiles
        {
            get => GameMgr.Instance.maxTurn;
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
        private GameObject victoryCanva;

        [SerializeField]
        private KeyCode nextPhase = KeyCode.Return, menu = KeyCode.Escape;

        [SerializeField]
        private Button validateButton;

        public bool EnableValidateBtn
        {
            set => validateButton.interactable = value;
        }

        [SerializeField]
        private Button undoButton;

        [SerializeField]
        private Button redoButton;

        #region Unity events

        protected override void Awake()
        {
            base.Awake();
            SetUpGui();
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

        #endregion

        private void SetUpGui()
        {
            for (int i = 0; i < GameMgr.Instance.NbPlayers; i++)
            {
                _guis[i] = Instantiate(playerPrefab, transform);
                RectTransform rt = _guis[i].GetComponent<RectTransform>();

                rt.pivot = Vector2.one;
                rt.anchorMin = Vector2.one;
                rt.anchorMax = Vector2.one;
                rt.anchoredPosition = new(-10, -10 - 110 * i);

                foreach (MeshRenderer mr in _guis[i].GetComponentsInChildren<MeshRenderer>())
                    mr.material.color = GameMgr.Instance.players[i].ID.GetColor();

                _guis[i].transform.GetChild(0).GetComponent<Text>().text = $"Player {i}";
                _guis[i].transform.GetChild(1).GetComponent<Image>().color = GameMgr.Instance.players[i].ID.GetColor();
            }
        }

        public void UnloadSetUp()
        {
            //Enlever les joueurs affiché qui n'existe pas
            for (int i = GameMgr.Instance.NbPlayers; i < 4; i++)
                Destroy(this.gameObject.transform.GetChild(i + 5).gameObject);
        }

        public void UpdateCurrentPlayer()
        {
            for (int i = 0; i < GameMgr.Instance.NbPlayers; i++)
            {
                _guis[i].GetComponent<Image>().color = i == GameMgr.Instance.ActualPlayerIndex ?
                    Color.white :
                    GameMgr.Instance.players[i].Eliminated ?
                        new(1, 0, 0, .25f) :
                        new(.75f, .75f, .75f);

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
            }
        }

        public void UpdatePlayersBuild()
        {
            for (int i = 0; i < GameMgr.Instance.NbPlayers; i++)
            {
                _guis[i].transform.GetChild(2).GetComponentInChildren<Text>().text =
                    GameMgr.Instance.players[i].nbBarrack.ToString();
                _guis[i].transform.GetChild(3).GetComponentInChildren<Text>().text =
                    GameMgr.Instance.players[i].nbTowers.ToString();
                _guis[i].transform.GetChild(4).GetComponentInChildren<Text>().text =
                    GameMgr.Instance.players[i].nbTemple.ToString();
            }
        }

        public void UpdateCurrentPlayerBuild()
        {
            currentPlayerBuildCount[0].text = GameMgr.Instance.actualPlayer.nbBarrack.ToString();
            currentPlayerBuildCount[1].text = GameMgr.Instance.actualPlayer.nbTowers.ToString();
            currentPlayerBuildCount[2].text = GameMgr.Instance.actualPlayer.nbTemple.ToString();
        }

        public void Next()
        {
            (GameMgr.Instance.actualPhase switch
            {
                TurnPhase.SelectCells => (Action<bool>) TilesMgr.Instance.ValidateTile,
                TurnPhase.PlaceBuilding => TilesMgr.Instance.ValidateBuild
            }).Invoke(true);
        }

        public void Phase1()
        {
            undoButton.interactable = GameMgr.Instance.CanUndo;
            redoButton.interactable = GameMgr.Instance.CanRedo;
            UpdateGui();
            
            TilesMgr.Instance.SetFeedForwards1();
            
            CurrentTile.SetActive(true);
            
            ChangeTileColor(GameMgr.Instance.actualChunk);
            
            currentTile.SetActive(true);
            currentTile.transform.GetChild(0).gameObject.SetActive(true);
            builds.SetActive(false);
        }

        public void ChangeTileColor(Chunk chunk)
        {
            MeshRenderer mr = currentTile.transform.GetComponentInChildren<MeshRenderer>(true);
            
            mr.materials[0].color = chunk.Coords[1].ActualBiome.GetColor();
            mr.materials[2].color = Biomes.Volcano.GetColor();
            mr.materials[3].color = chunk.Coords[2].ActualBiome.GetColor();
        }

        public void Phase2()
        {
            undoButton.interactable = GameMgr.Instance.CanUndo;
            redoButton.interactable = GameMgr.Instance.CanRedo;
            UpdateGui();
            currentTile.SetActive(false);
            builds.SetActive(true);

            foreach (MeshRenderer mr in builds.GetComponentsInChildren<MeshRenderer>())
                mr.material.color = GameMgr.Instance.actualPlayer.ID.GetColor();

            NbTiles--;
            UpBuild(0);
        }

        private void UpdateGui()
        {
            UpdateCurrentPlayer();
            UpdatePlayersBuild();
            UpdateCurrentPlayerBuild();
        }

        public void Undo()
        {
            GameManagment.Coup coup = GameMgr.Instance.Undo();

            (GameMgr.Instance.actualPhase switch
            {
                TurnPhase.SelectCells => (Action<GameManagment.Coup>) UndoPhase1,
                TurnPhase.PlaceBuilding => UndoPhase2
            }).Invoke(coup);


            if (GameMgr.Instance.actualPlayer is AI)
            {
                for(int i = 0; i < 2; i++)
                {
                    coup = GameMgr.Instance.Undo();

                    (GameMgr.Instance.actualPhase switch
                    {
                        TurnPhase.SelectCells => (Action<GameManagment.Coup>)UndoPhase1,
                        TurnPhase.PlaceBuilding => UndoPhase2
                    }).Invoke(coup);
                }
            }
        }

        private void UndoPhase1(GameManagment.Coup c) => TilesMgr.Instance.RemoveTile(c.positions[0]);

        private void UndoPhase2(GameManagment.Coup c) => TilesMgr.Instance.RemoveBuild(c.positions);

        public void Redo()
        {
            GameManagment.Coup coup = GameMgr.Instance.Redo();

            (GameMgr.Instance.actualPhase switch
            {
                TurnPhase.PlaceBuilding => (Action<GameManagment.Coup>) RedoPhase1,
                TurnPhase.SelectCells => RedoPhase2,
                TurnPhase.IAPlays => RedoPhase2
            }).Invoke(coup);
        }

        private void RedoPhase1(GameManagment.Coup coup)
        {
            // ReSharper disable once PossibleInvalidOperationException
            TilesMgr.Instance.ReputTile(coup.positions[0], (Rotation) coup.rotation);
            if (GameMgr.Instance.actualPhase == TurnPhase.PlaceBuilding)
                TilesMgr.Instance.SetFeedForwards2(Building.Barrack);
            else
                CurrentTile.SetActive(true);

        }

        private void RedoPhase2(GameManagment.Coup coup)
        {
            if (GameMgr.Instance.players[coup.playerIndex].Eliminated)
            {
                RedoPhase1(coup);
            }
            else
            {
                TilesMgr.Instance.ReputBuild(coup.positions[0], coup.building[0], GameMgr.Instance.PreviousPlayer);
            }

            TilesMgr.Instance.SetFeedForwards1();
        }

        public void ToggleMenu()
        {
            menuCanva.SetActive(!menuCanva.activeSelf);
            EnableScript();
        }

        public void ToggleVictory()
        {
            EnableScript();
            victoryCanva.SetActive(true);
        }

        public void EnableScript()
        {
            CameraMgr.Instance.enabled = !CameraMgr.Instance.enabled;
            Instance.enabled = !Instance.enabled;
            TilesMgr.Instance.enabled = !TilesMgr.Instance.enabled;
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