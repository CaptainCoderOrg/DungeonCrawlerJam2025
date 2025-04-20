using System.Collections;

using UnityEngine;
using UnityEngine.Events;

namespace CaptainCoder.Unity.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class MusicTrackController : MonoBehaviour
    {
        [SerializeField] private AudioSource _introSource;
        [SerializeField] private AudioSource _ambienceSource;
        [SerializeField]
        private MusicTrackManager _trackManager;
        [SerializeField] private AudioClip _introClip;
        [SerializeField] private AudioClip _ambienceClip;
        public bool BecomeTrackOnStart = true;
        public UnityEvent OnFadeOutFinished;
        [field: SerializeField]
        public float FadeDuration { get; set; } = 5f;
        private AudioSource _audioSource;
        public AudioClip Clip => _audioSource?.clip;
        public bool IsPlaying => _audioSource == null ? false : _audioSource.isPlaying;
        void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            if (_introClip != null)
            {
                _introSource.clip = _introClip;
                _introSource.loop = false;
                _audioSource.playOnAwake = false;
                _audioSource.Stop();
            }
            if (_ambienceClip != null)
            {
                Debug.Log("Ambience detected");
                _ambienceSource.clip = _ambienceClip;
            }
        }

        void Start()
        {
            if (BecomeTrackOnStart)
            {
                _trackManager.Track = this;
            }
        }

        private IEnumerator ChangeVolume(float startVolume, float endVolume, UnityEvent callback = null)
        {
            if (_ambienceClip != null)
            {
                _ambienceSource.Play();
            }
            if (!_audioSource.isPlaying && _introClip == null)
            {
                _audioSource.Play();

            }
            else if (_introClip != null)
            {
                _introSource.Play();
                _audioSource.PlayDelayed(_introClip.length);
            }
            float startTime = Time.time;
            float percent = 0;
            while (percent < 1)
            {
                percent = Mathf.Clamp01((Time.time - startTime) / FadeDuration);
                _audioSource.volume = Mathf.Lerp(startVolume, endVolume, percent);
                _introSource.volume = Mathf.Lerp(startVolume, endVolume, percent);
                _ambienceSource.volume = Mathf.Lerp(startVolume, endVolume, percent);
                yield return null;
            }
            _audioSource.volume = endVolume;
            callback?.Invoke();
        }

        public void FadeIn()
        {
            StopAllCoroutines();
            StartCoroutine(ChangeVolume(0, 1));
        }

        public void FadeOut()
        {
            StopAllCoroutines();
            StartCoroutine(ChangeVolume(_audioSource.volume, 0, OnFadeOutFinished));
        }
    }
}