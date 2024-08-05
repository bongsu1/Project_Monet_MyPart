using UnityEngine;

namespace BS
{
    public enum Voice { Soliloquy, Conversation }

    public class NPCVoice : MonoBehaviour
    {
        [SerializeField] AudioSource voice;
        public AudioSource Voice { get { return voice; } }

        public void PlayVoice(string path)
        {
            AudioClip clip = Manager.Resource.Load<AudioClip>($"Scenario/Sounds/Voice/{path}");
            if (clip == null)
                return;

            voice.clip = clip;
            voice.Play();
        }
    }
}
