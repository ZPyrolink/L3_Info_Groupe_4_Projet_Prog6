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
        private Dropdown playerType;

        private Image _background;

        public bool Enabled
        {
            set => addButton.interactable = value;
        }

        public bool Registered => !addButton.gameObject.activeSelf;

        public Difficulty Ai => (Difficulty) playerType.value - 1; 

        private void Start()
        {
            _background = GetComponent<Image>();

            Difficulty[] difficulties = (Difficulty[]) Enum.GetValues(typeof(Difficulty));
            playerType.options.AddRange(difficulties.Select(d => new Dropdown.OptionData(d.ToString())));
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