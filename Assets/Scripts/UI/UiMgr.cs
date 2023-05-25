using System;
using System.Collections.Generic;
using System.Linq;

using Taluva.Controller;
using Taluva.Model;
using Taluva.Model.AI;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

using Utils;

using Wrapper;

using static Taluva.Controller.GameManagment;

namespace UI
{
    public class UiMgr : MonoBehaviourMgr<UiMgr>
    {
        [SerializeField]
        private Text uiNbTiles;

        private const string NB_TILES_PLACEHOLDER = "%nb% tuiles restantes";

        private int NbTiles
        {
            get => GameMgr.Instance.KeepingTiles;
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

        public bool inMenu { get; private set; }

        [SerializeField]
        private Button undoButton;

        public bool InteractiveValidate
        {
            set => validateButton.interactable = value;
        }

        public bool InteractiveUndo
        {
            set => undoButton.interactable = value;
        }

        public bool InteractiveRedo
        {
            set => redoButton.interactable = value;
        }

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
            if (!inMenu)
            {
                if (Input.GetKeyDown(nextPhase))
                    Next();
            }

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

                foreach ((MeshRenderer mr, Building b) in _guis[i].GetComponentsInChildren<MeshRenderer>()
                             .Select(static (mr, i) => (mr, (Building)i + 1)))
                {
                    mr.materials[TilesMgr.BuildOwnerMatIndex[b]].color = GameMgr.Instance.Players[i].IdColor;
                }

                if (GameMgr.Instance.Players[i] is AI)
                    _guis[i].transform.GetChild(0).GetComponent<Text>().text = $"Player {i}";
                else
                    _guis[i].transform.GetChild(0).GetComponent<Text>().text = GameMgr.Instance.Players[i].Name;

                _guis[i].transform.GetChild(1).GetComponent<Image>().color = GameMgr.Instance.Players[i].ID.GetColor();

                if (GameMgr.Instance.Players[i] is AI)
                {
                    _guis[i].transform.GetChild(5).gameObject.SetActive(true);
                    string s = "AI ";
                    switch (((AI)GameMgr.Instance.Players[i]).Difficulty)
                    {
                        case Difficulty.BadPlayer:
                            s += "E";
                            break;
                        case Difficulty.Normal:
                            s += "M";
                            break;
                        case Difficulty.SkillIssue:
                            s += "H";
                            break;
                    }
                    _guis[i].transform.GetChild(5).GetComponent<Text>().text = s;
                }

            }
        }

        public void UnloadSetUp()
        {
            //Enlever les joueurs affich� qui n'existe pas (utilis� pour le load)
            for (int i = GameMgr.Instance.NbPlayers; i < 4; i++)
                Destroy(this.gameObject.transform.GetChild(i + 5).gameObject);
        }

        public void UpdateCurrentPlayer()
        {
            for (int i = 0; i < GameMgr.Instance.NbPlayers; i++)
            {
                _guis[i].GetComponent<Image>().color = i == GameMgr.Instance.CurrentPlayerIndex ?
                    Color.white :
                    GameMgr.Instance.Players[i].Eliminated ?
                        new(1, 0, 0, .25f) :
                        new(.75f, .75f, .75f);

                if (!(GameMgr.Instance.Players[i] is AI))
                    _guis[i].transform.GetChild(0).gameObject.GetComponent<Text>().text = GameMgr.Instance.Players[i].Name;

                foreach (Animator anim in _guis[i].GetComponentsInChildren<Animator>())
                    if (GameMgr.Instance.CurrentPlayerIndex == i)
                    {
                        anim.enabled = true;
                    } else
                    {
                        anim.enabled = false;
                    }
            }
        }

        public void UpdatePlayersBuild()
        {
            for (int i = 0; i < GameMgr.Instance.NbPlayers; i++)
            {
                _guis[i].transform.GetChild(2).GetComponentInChildren<Text>().text =
                    GameMgr.Instance.Players[i].NbBarrack.ToString();
                _guis[i].transform.GetChild(3).GetComponentInChildren<Text>().text =
                    GameMgr.Instance.Players[i].NbTowers.ToString();
                _guis[i].transform.GetChild(4).GetComponentInChildren<Text>().text =
                    GameMgr.Instance.Players[i].NbTemple.ToString();
            }
        }

        public void UpdateCurrentPlayerBuild()
        {
            currentPlayerBuildCount[0].text = GameMgr.Instance.CurrentPlayer.NbBarrack.ToString();
            currentPlayerBuildCount[1].text = GameMgr.Instance.CurrentPlayer.NbTowers.ToString();
            currentPlayerBuildCount[2].text = GameMgr.Instance.CurrentPlayer.NbTemple.ToString();
        }

        public void Next()
        {
            (GameMgr.Instance.CurrentPhase switch
            {
                TurnPhase.SelectCells => (Action<bool>)TilesMgr.Instance.ValidateTile,
                TurnPhase.PlaceBuilding => TilesMgr.Instance.ValidateBuild
            }).Invoke(true);
        }

        public void MoveSuggestion()
        {
            AIRandom ai = new(GameMgr.Instance.CurrentPlayer.ID, GameMgr.Instance);

            switch (GameMgr.Instance.CurrentPhase)
            {
                case TurnPhase.SelectCells:
                    PointRotation p = ai.PlayChunk();
                    TilesMgr.Instance.PreviewTile(p);
                    break;
                case TurnPhase.PlaceBuilding:
                    (Building b, Vector2Int pos) = ai.PlayBuild();
                    TilesMgr.Instance.PreviewBuild(pos, b);
                    validateButton.interactable = true;
                    break;
            }

            EventSystem.current.SetSelectedGameObject(validateButton.gameObject);
        }

        public void Phase1()
        {
            UpdateGui();
            InteractiveValidate = false;
            TilesMgr.Instance.SetFeedForwards1();

            CurrentTile.SetActive(true);

            ChangeTileColor(GameMgr.Instance.CurrentChunk);

            currentTile.SetActive(true);
            currentTile.transform.GetChild(0).gameObject.SetActive(true);
            builds.SetActive(false);
        }

        public void ChangeTileColor(Chunk chunk) => TilesMgr.ChangeTileColor(chunk,
            currentTile.transform.GetComponentInChildren<MeshRenderer>(true),
            TilesMgr.Instance.Materials);

        public void UpdateTiles() => NbTiles = NbTiles;

        public void Phase2()
        {
            UpdateGui();
            InteractiveValidate = false;
            currentTile.SetActive(false);
            builds.SetActive(true);

            foreach ((MeshRenderer mr, Building b) in builds.transform.Cast<Transform>()
                         .Select((t, i) => (t.GetComponentInChildren<MeshRenderer>(), (Building)i + 1)))
            {
                mr.materials[TilesMgr.BuildOwnerMatIndex[b]].color = GameMgr.Instance.CurrentPlayer.IdColor;
            }

            UpdateTiles();
            UpBuild(0);
        }

        private void UpdateGui()
        {
            UpdateCurrentPlayer();
            UpdatePlayersBuild();
            UpdateCurrentPlayerBuild();

            undoButton.interactable = GameMgr.Instance.CanUndo;
            redoButton.interactable = GameMgr.Instance.CanRedo;
        }

        public void Undo()
        {
            InteractiveUndo = false;
            InteractiveRedo = false;
            TilesMgr.Instance.ClearFeedForward();
            if (TilesMgr.Instance.CurrentPreviewsNotNull)
                TilesMgr.Instance.ClearCurrentPreviews();

            GameManagment.Coup coup = GameMgr.Instance.historic[GameMgr.Instance.historic.Index];
            Vector2Int[] positionsReput = new Vector2Int[] {};
            if (coup.chunk != null)
            {
                UndoProps(coup);
                positionsReput = GameMgr.Instance.gameBoard.GetChunksCoords(coup.positions[0], (Rotation)coup.rotation);
            }
            coup = GameMgr.Instance.Undo();
            if(coup.chunk != null && coup.cells[0] != null)
                ReputProps(positionsReput);

            (GameMgr.Instance.CurrentPhase switch
            {
                TurnPhase.SelectCells => (Action<GameManagment.Coup>)UndoPhase1,
                TurnPhase.PlaceBuilding => UndoPhase2
            }).Invoke(coup);


            while (GameMgr.Instance.CurrentPlayer is AI)
            {
                coup = GameMgr.Instance.historic[GameMgr.Instance.historic.Index];
                if(coup.chunk != null)
                {
                    UndoProps(coup);
                    positionsReput = GameMgr.Instance.gameBoard.GetChunksCoords(coup.positions[0], (Rotation)coup.rotation);
                }
                coup = GameMgr.Instance.Undo();
                if(coup.chunk != null && coup.cells[0] != null)
                    ReputProps(positionsReput);

                (GameMgr.Instance.CurrentPhase switch
                {
                    TurnPhase.SelectCells => (Action<GameManagment.Coup>)UndoPhase1,
                    TurnPhase.PlaceBuilding => UndoPhase2
                }).Invoke(coup);
            }

            InteractiveUndo = GameMgr.Instance.CanUndo;
            InteractiveRedo = GameMgr.Instance.CanRedo;
        }

        private void UndoProps(Coup coup)
        {
            TilesMgr.Instance.ClearInformations(coup.positions[0], (Rotation)coup.rotation);
        }

        private void ReputProps(Vector2Int[] positions)
        {
            foreach (Vector2Int pos in positions)
            {
                Cell cell = GameMgr.Instance.gameBoard.WorldMap[pos];
                if (!TilesMgr.Instance.BiomeProps.ContainsKey(cell.CurrentBiome))
                    continue;

                Vector2Int coord = cell.position;
                Vector3 propsPos = TilesMgr.V2IToV3(coord);
                Vector3 propsRot = TilesMgr.V2IToEul(coord);

                Instantiate(TilesMgr.Instance.BiomeProps[cell.CurrentBiome], propsPos, Quaternion.Euler(propsRot), TilesMgr.Instance.PropsParent);
            }

        }

        private void UndoPhase1(GameManagment.Coup c)
        {
            TilesMgr.Instance.RemoveTile(c.positions[0]);
            if (c.building == null)
                return;
            for (int i = 0; i < c.building.Length; i++)
            {
                if (c.building[i] != Building.None)
                {
                    Player player = GameMgr.Instance.Players[0];
                    for (int j = 0; j < GameMgr.Instance.NbPlayers; j++)
                        if (GameMgr.Instance.Players[j].ID == c.cells[i].Owner)
                            player = GameMgr.Instance.Players[j];
                    TilesMgr.Instance.ReputBuild(c.positions[i], c.building[i], player);
                }
            }
        }

        private void UndoPhase2(GameManagment.Coup c) => TilesMgr.Instance.RemoveBuild(c.positions);

        public void Redo()
        {
            InteractiveUndo = false;
            InteractiveRedo = false;

            GameManagment.Coup c = GameMgr.Instance.historic[GameMgr.Instance.historic.Index + 1];
            if (c.chunk != null && !GameMgr.Instance.IsVoid(c.positions[0]))
                TilesMgr.Instance.ClearInformations(c.positions[0], (Rotation)c.rotation);

            GameManagment.Coup coup = GameMgr.Instance.Redo();

            (GameMgr.Instance.CurrentPhase switch
            {
                TurnPhase.PlaceBuilding => (Action<GameManagment.Coup>)RedoPhase1,
                TurnPhase.SelectCells => RedoPhase2,
                TurnPhase.IAPlays => RedoPhase2
            }).Invoke(coup);

            InteractiveUndo = GameMgr.Instance.CanUndo;
            InteractiveRedo = GameMgr.Instance.CanRedo;
        }

        private void RedoPhase1(GameManagment.Coup coup)
        {
            // ReSharper disable once PossibleInvalidOperationException
            TilesMgr.Instance.ReputTile(coup.positions[0], (Rotation)coup.rotation);
            if (GameMgr.Instance.CurrentPhase == TurnPhase.PlaceBuilding)
                TilesMgr.Instance.SetFeedForwards2(Building.Barrack);
            else
                CurrentTile.SetActive(true);
        }

        private void RedoPhase2(GameManagment.Coup coup)
        {
            if (GameMgr.Instance.Players[coup.playerIndex].Eliminated)
            {
                RedoPhase1(coup);
            } else
            {
                for (int i = 0; i < coup.positions.Length; i++)
                {
                    TilesMgr.Instance.ReputBuild(coup.positions[i], coup.building[i], GameMgr.Instance.PreviousPlayer);
                }
            }

            TilesMgr.Instance.SetFeedForwards1();
        }

        public void ToggleMenu()
        {
            if (!menuCanva.activeSelf && !SaveMgr.Instance.gameObject.activeSelf
                                      && !SettingsMgr.Instance.gameObject.activeSelf)
                EnableScript();

            if (menuCanva.activeSelf && !SaveMgr.Instance.gameObject.activeSelf
                                     && !SettingsMgr.Instance.gameObject.activeSelf)
                EnableScript();

            menuCanva.SetActive(!menuCanva.activeSelf);
            SaveMgr.Instance.gameObject.SetActive(false);
            SettingsMgr.Instance.gameObject.SetActive(false);
        }

        public void ToggleVictory()
        {
            EnableScript();
            victoryCanva.SetActive(true);
        }

        public void EnableScript()
        {
            CameraMgr.Instance.enabled = !CameraMgr.Instance.enabled;
            inMenu = !inMenu;
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

            TilesMgr.Instance.SetFeedForwards2((Building)i + 1);

            child = builds.transform.GetChild(i).GetComponent<RectTransform>();
            child.anchoredPosition = child.anchoredPosition.With(y: 20);
            anim = child.GetComponentInChildren<Animator>();
            anim.enabled = true;
        }
    }
}