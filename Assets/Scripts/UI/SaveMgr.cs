using System;
using Taluva.Controller;
using UI;

using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace UI
{
    public class SaveMgr : MonoBehaviourMgr<SaveMgr>
    {
        [SerializeField]
        private GameObject menuCanva;

        public void Start()
        {
            this.gameObject.SetActive(false);
        }

        public void Quit() => ToggleMenu();

        public void ToggleMenu()
        {
            menuCanva.SetActive(!menuCanva.activeSelf);
            this.gameObject.SetActive(!menuCanva.activeSelf);
        }
    }
}
