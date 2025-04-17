
using System;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.CrawlingMode
{
    [CreateAssetMenu(menuName = "OptionsSettings")]
    public class OptionsSettings : ObservableSO
    {
        [field: SerializeField] private float _uiScaling = 1;
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

        public override void OnBeforeEnterPlayMode()
        {
            base.OnBeforeEnterPlayMode();
            OnUIScalingChanged = null;
        }
    }
}