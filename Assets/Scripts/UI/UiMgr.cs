using System;

using Taluva.Model;

using UnityEngine;
using UnityEngine.UI;

using UnityUtils.GameObjects;

using Utils;

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
            set => currentPlayerIndex = value >= players.Length ? 0 : value;
        }

        private Player CurrentPlayer => players[currentPlayerIndex];

        [SerializeField]
        private GameObject playerPrefab;

        private readonly GameObject[] guis = new GameObject[4];

        [SerializeField]
        private Text[] currentPlayerBuild;

        [SerializeField]
        private GameObject tile, builds;

        [Header("Debug keys")]
        [SerializeField]
        private KeyCode phase1 = KeyCode.Keypad1;

        [SerializeField]
        private KeyCode phase2 = KeyCode.Keypad2,
            nextPlayer = KeyCode.KeypadEnter,
            nextPhase = KeyCode.Return;

        private byte _phase;
        private byte Phase
        {
            get => _phase;
            set
            {
                if (value > 2)
                {
                    NextPlayer();
                    _phase = 1;
                }
                else
                {
                    _phase = value;
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
        }

        // Start is called before the first frame update
        private void OnGUI()
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (guis[i] is null)
                    guis[i] = (GameObject) playerPrefab.InstantiateAt(parent: transform);

                guis[i].GetComponent<Image>().color = i == currentPlayerIndex ? Color.white : new(.75f, .75f, .75f);

                guis[i].transform.GetChild(0).GetComponent<Text>().text = players[i].Name;
                guis[i].transform.GetChild(1).GetComponent<Image>().color = players[i].Color;

                guis[i].transform.GetChild(2).GetComponentInChildren<Text>().text = players[i].Builds[0].ToString();
                guis[i].transform.GetChild(3).GetComponentInChildren<Text>().text = players[i].Builds[1].ToString();
                guis[i].transform.GetChild(4).GetComponentInChildren<Text>().text = players[i].Builds[2].ToString();

                RectTransform rt = guis[i].GetComponent<RectTransform>();

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
        }

        private void NextPlayer() => CurrentPlayerIndex++;
    }
}