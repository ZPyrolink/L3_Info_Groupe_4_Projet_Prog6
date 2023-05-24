using UnityEngine;
using Taluva.Model;
using UnityEngine.UI;

namespace UI
{
    public class CameraScript : MonoBehaviour
    {
        [SerializeField]
        private Toggle audio;
        [SerializeField]
        private AudioSource audioSource;

        public void Start()
        {
            audioSource.enabled = Settings.allowSound;
            audio.isOn = Settings.allowSound;
        }

    }
}
