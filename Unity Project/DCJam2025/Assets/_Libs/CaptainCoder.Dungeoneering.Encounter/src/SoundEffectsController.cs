using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    public class SoundEffectsController : MonoBehaviour
    {
        public AudioSource[] SFXBanks;
        public int Index = 0;

        internal void Play(AudioClip[] clips, float volume)
        {
            if (Index >= SFXBanks.Length) { Index = 0; }
            AudioSource audioSource = SFXBanks[Index++];
            if (!audioSource.isPlaying)
            {
                audioSource.volume = volume;
                audioSource.clip = clips[Random.Range(0, clips.Length)];
                audioSource.Play();
            }
        }
    }
}