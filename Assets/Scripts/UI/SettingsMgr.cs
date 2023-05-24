using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Taluva.Model;
using Utils;

namespace UI
{
    public class SettingsMgr : MonoBehaviourMgr<SettingsMgr>
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
            this.gameObject.SetActive(!this.gameObject.activeSelf);
        }
    }
}
