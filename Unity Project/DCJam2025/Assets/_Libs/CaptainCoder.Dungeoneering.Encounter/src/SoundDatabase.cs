using System;
using System.Collections.Generic;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    [CreateAssetMenu(menuName = "SoundDatabase")]
    public class SoundDatabase : ObservableSO
    {
        private SoundEffectsController _sfxController;
        public SoundEffectsController ControllerPrefab;
        public SoundEffectsController SFX
        {
            get
            {
                _sfxController = _sfxController == null ? (_sfxController = FindFirstObjectByType<SoundEffectsController>()) : _sfxController;
                if (_sfxController == null)
                {
                    _sfxController = Instantiate(ControllerPrefab);
                    DontDestroyOnLoad(_sfxController);
                }
                return _sfxController;
            }
        }

        public SoundEntry[] Entries;
        private readonly Dictionary<string, SoundEntry> _clips = new();

        public void Play(string key)
        {
            if (Time.timeSinceLevelLoad < 0.25) { return; }
            if (SFX != null && _clips.TryGetValue(key, out SoundEntry entry))
            {
                SFX.Play(entry.Clips, entry.Volume);
            }
            else
            {
                Debug.Log($"Could not play SFX '{key}'");
            }
        }

        public override void OnAfterEnterPlayMode()
        {
            base.OnAfterEnterPlayMode();
            _clips.Clear();
            foreach (SoundEntry entry in Entries)
            {
                _clips[entry.Key] = entry;
            }
        }

        protected override void OnExitPlayMode()
        {
            base.OnExitPlayMode();
            _sfxController = null;
        }
    }

    [Serializable]
    public struct SoundEntry
    {
        public string Key;
        public AudioClip[] Clips;
        public float Volume;
    }
}