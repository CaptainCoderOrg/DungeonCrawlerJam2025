
using System;

using CaptainCoder.Dungeoneering.Encounter;

using NaughtyAttributes;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.CrawlingMode
{
    [CreateAssetMenu(menuName = "OptionsSettings")]
    public class OptionsSettings : ObservableSO
    {
        [field: SerializeField] public bool MapOpen { get; set; } = false;
        [field: SerializeField] public bool HasBeenOpen = false;
        [field: SerializeField] public bool HasInitialized = false;
        [field: SerializeField] private float _uiScaling = 1;
        [field: Expandable][field: SerializeField] public EncounterSettingsData EncounterSettings { get; private set; }
        public float UIScaling
        {
            get => _uiScaling;
            set
            {
                _uiScaling = value;
                OnUIScalingChanged?.Invoke(_uiScaling);
            }
        }

        public event Action<float> OnUIScalingChanged;

        public void Initialize()
        {
            Resolution resolution = Screen.currentResolution;
            if (resolution.width <= 2000)
            {
                UIScaling = 1;
            }
            else if (resolution.width <= 3000)
            {
                UIScaling = 1.5f;
            }
            else
            {
                UIScaling = 2f;
            }
            HasInitialized = true;
        }

        public override void OnBeforeEnterPlayMode()
        {
            base.OnBeforeEnterPlayMode();
            OnUIScalingChanged = null;
            HasInitialized = false;
            HasBeenOpen = false;
        }
    }
}