using UnityEngine;

using Utils;
using Wrapper;

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

        public void Save(string path) => GameMgr.Instance.Save(path);

        public void Quit() => ToggleMenu();

        public void ToggleMenu()
        {
            menuCanva.SetActive(!menuCanva.activeSelf);
            this.gameObject.SetActive(!this.gameObject.activeSelf);
        }
    }
}
