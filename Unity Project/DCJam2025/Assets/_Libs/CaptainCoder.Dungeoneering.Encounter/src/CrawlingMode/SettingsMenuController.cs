using CaptainCoder.Dungeoneering.CrawlingMode;
using CaptainCoder.Unity.Assertions;

using TMPro;

using UnityEngine;
using UnityEngine.UI;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class SettingsMenuController : MonoBehaviour
    {
        [AssertIsSet][SerializeField] private OptionsSettings _settings;

        [AssertIsSet][SerializeField] private TextMeshProUGUI _uiScalingLabel;
        [AssertIsSet][SerializeField] private Slider _uiScalingSlider;

        void Start()
        {
            _settings.OnUIScalingChanged += HandleUIScalingChanged;
            HandleUIScalingChanged(_settings.UIScaling);
        }

        private void HandleUIScalingChanged(float value)
        {
            _uiScalingLabel.text = $"UI Scaling: {(int)(value * 100)}%";
            _uiScalingSlider.value = value;
        }

        public void SetUIScaling(float value) => _settings.UIScaling = value;
    }
}