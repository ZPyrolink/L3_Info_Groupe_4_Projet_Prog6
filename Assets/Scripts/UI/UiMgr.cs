using UnityEngine;
using UnityEngine.UI;

using UnityUtils.GameObjects;

using Utils;

namespace UI
{
    public class UiMgr : MonoBehaviour
    {
        [SerializeField]
        private GameObject light;
        
        [System.Serializable]
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
            new("P1", ColorUtils.FromInt(0xFFD20F28), new[] { 0, 1, 2 }),
            new("P2", ColorUtils.FromInt(0xFF6C935E), new[] { 1, 1, 2 }),
            new("P3", ColorUtils.FromInt(0xFF4855B7), new[] { 2, 1, 2 }),
            new("P4", ColorUtils.FromInt(0xFFFFD700), new[] { 3, 1, 2 })
        };

        [SerializeField]
        private int currentPlayerIndex;

        private Player CurrentPlayer => players[currentPlayerIndex];

        [SerializeField]
        private GameObject playerPrefab;

        private GameObject[] guis = new GameObject[4];

        [SerializeField]
        private Text[] currentPlayerBuild;

        private void NextPlayer()
        {
            if (++currentPlayerIndex >= players.Length)
                currentPlayerIndex = 0;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                NextPlayer();
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
    }
}