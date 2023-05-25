using System;
using System.Linq;

using Taluva.Model;

using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenu
{
    public class PlayerSelection : MonoBehaviour
    {
        [SerializeField]
        private Button addButton;

        [SerializeField]
        private Button removeButton;

        [SerializeField]
        public Dropdown playerType;

        private Image _background;

        [SerializeField]
        public InputField PlayerName;

        public bool Enabled
        {
            set => addButton.interactable = value;
        }

        public bool Registered => !addButton.gameObject.activeSelf;

        public Difficulty Ai => (Difficulty) playerType.value - 1; 

        private void Start()
        {
            _background = GetComponent<Image>();
            playerType.options.Add(new Dropdown.OptionData("Ai Easy"));
            playerType.options.Add(new Dropdown.OptionData("Ai Medium"));
            playerType.options.Add(new Dropdown.OptionData("Ai Hard"));
        }

        private void Update()
        {
            if(playerType.value == 0)
            {
                PlayerName.gameObject.SetActive(true);
            }
            else
            {
                PlayerName.gameObject.SetActive(false);
            }
        }

        public void AddPlayer()
        {
            addButton.gameObject.SetActive(false);
            removeButton.gameObject.SetActive(true);
            playerType.gameObject.SetActive(true);
            _background.color = Color.white;
        }

        public void RemovePlayer()
        {
            addButton.gameObject.SetActive(true);
            removeButton.gameObject.SetActive(false);
            playerType.gameObject.SetActive(false);
            _background.color = Color.gray;
        }
    }
}