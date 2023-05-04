using System;

using Taluva.Model;

using UnityEngine;
using UnityEngine.UI;

using UnityUtils.GameObjects;

using Utils;

using GameObject = UnityEngine.GameObject;
using Random = UnityEngine.Random;

namespace UI
{
    public class UiMgr : MonoBehaviour
    {
        [Serializable]
        public class Player
        {
            [SerializeField]
            private string name;

            public string Name => name;

            [SerializeField]
            private Color color;

            public Color Color => color;

            [SerializeField]
            private int[] builds;

            public int[] Builds => builds;

            public Player(string name, Color color, int[] builds)
            {
                this.name = name;
                this.color = color;
                this.builds = builds;
            }
        }

        [SerializeField]
        private Player[] players =
        {
            new("P1", ColorUtils.FromInt((int) PlayerColor.Red), new[] { 0, 1, 2 }),
            new("P2", ColorUtils.FromInt((int) PlayerColor.Green), new[] { 1, 1, 2 }),
            new("P3", ColorUtils.FromInt((int) PlayerColor.Blue), new[] { 2, 1, 2 }),
            new("P4", ColorUtils.FromInt((int) PlayerColor.Yellow), new[] { 3, 1, 2 })
        };

        [SerializeField]
        private int currentPlayerIndex;

        public int CurrentPlayerIndex
        {
            get => currentPlayerIndex;
            set
            {
                if (value >= players.Length)
                    currentPlayerIndex = 0;
                else if (value < 0)
                    currentPlayerIndex = players.Length - 1;
                else
                    currentPlayerIndex = value;
            }
        }

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

        private Player CurrentPlayer => players[currentPlayerIndex];

        [SerializeField]
        private GameObject playerPrefab;

        private readonly GameObject[] _guis = new GameObject[4];

        [SerializeField]
        private Text[] currentPlayerBuild;

        [SerializeField]
        private GameObject tile, builds;

        [SerializeField]
        private GameObject menuCanva; 

        [Header("Debug keys")]
        [SerializeField]
        private KeyCode phase1 = KeyCode.Keypad1;

        [SerializeField]
        private KeyCode phase2 = KeyCode.Keypad2,
            nextPlayer = KeyCode.KeypadEnter,
            nextPhase = KeyCode.Return,
            menu = KeyCode.Escape;

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
            NbTiles = nbTilesPerPlayers * players.Length;
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
            for (int i = 0; i < players.Length; i++)
            {
                _guis[i] ??= Instantiate(playerPrefab, transform);

                _guis[i].GetComponent<Image>().color = i == currentPlayerIndex ? Color.white : new(.75f, .75f, .75f);

                _guis[i].transform.GetChild(0).GetComponent<Text>().text = players[i].Name;
                _guis[i].transform.GetChild(1).GetComponent<Image>().color = players[i].Color;

                _guis[i].transform.GetChild(2).GetComponentInChildren<Text>().text = players[i].Builds[0].ToString();
                _guis[i].transform.GetChild(3).GetComponentInChildren<Text>().text = players[i].Builds[1].ToString();
                _guis[i].transform.GetChild(4).GetComponentInChildren<Text>().text = players[i].Builds[2].ToString();

                RectTransform rt = _guis[i].GetComponent<RectTransform>();

                rt.pivot = Vector2.one;
                rt.anchorMin = Vector2.one;
                rt.anchorMax = Vector2.one;
                rt.anchoredPosition = new(-10, -10 - 110 * i);
            }

            currentPlayerBuild[0].text = CurrentPlayer.Builds[0].ToString();
            currentPlayerBuild[1].text = CurrentPlayer.Builds[1].ToString();
            currentPlayerBuild[2].text = CurrentPlayer.Builds[2].ToString();
        }

        #endregion

        public void Next() => Phase++;

        private void Phase1()
        {
            int[] values = (int[]) Enum.GetValues(typeof(BiomeColor));
            MeshRenderer mr = tile.transform.GetComponentInChildren<MeshRenderer>();
            mr.materials[1].color = ColorUtils.FromInt(values[Random.Range(0, values.Length - 1)]);
            mr.materials[2].color = ColorUtils.FromInt(values[Random.Range(0, values.Length - 1)]);
            mr.materials[3].color = ColorUtils.FromInt(values[Random.Range(0, values.Length - 1)]);
            tile.SetActive(true);
            builds.SetActive(false);
        }

        private void Phase2()
        {
            tile.SetActive(false);
            builds.SetActive(true);
            NbTiles--;
        }

        public void Undo() => Phase--;
        public void Redo() => Phase++;

        private void NextPlayer() => CurrentPlayerIndex++;
        private void PreviousPlayer() => CurrentPlayerIndex--;

        public void ToggleMenu()
        {
            menuCanva.SetActive(!menuCanva.activeSelf);
        }
    }
}