using System;

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
        [AssertIsSet][SerializeField] private ToggleablePanel _toggleablePanel;
        [AssertIsSet][SerializeField] private TextMeshProUGUI _uiScalingLabel;
        [AssertIsSet][SerializeField] private Slider _uiScalingSlider;
        [AssertIsSet][SerializeField] private TextMeshProUGUI _enemySpeedLabel;
        [AssertIsSet][SerializeField] private Slider _enemySpeedSlider;
        [AssertIsSet][SerializeField] private Toggle _autoConfirmToggle;

        void Start()
        {
            _settings.OnUIScalingChanged += HandleUIScalingChanged;
            _settings.EncounterSettings.OnEnemySpeedChanged += HandleEnemySpeedChange;
            _settings.EncounterSettings.OnAutoConfirmChanged += HandleConfirmChanged;
            HandleUIScalingChanged(_settings.UIScaling);
            HandleEnemySpeedChange(_settings.EncounterSettings.EnemySpeedMultiplier);
            HandleConfirmChanged(_settings.EncounterSettings.AutoConfirmEnemy);
            if (!_settings.HasBeenOpen)
            {
                _toggleablePanel.IsEnabled = true;
                _settings.HasBeenOpen = true;
            }
        }

        private void HandleConfirmChanged(bool value) => _autoConfirmToggle.isOn = value;
        public void SetAutoConfirm(bool value) => _settings.EncounterSettings.AutoConfirmEnemy = value;

        private void HandleEnemySpeedChange(float value)
        {
            _enemySpeedLabel.text = $"Enemy Speed: {value:#.#}x";
            _enemySpeedSlider.value = value;
        }

        private void HandleUIScalingChanged(float value)
        {
            _uiScalingLabel.text = $"UI Scaling: {(int)(value * 100)}%";
            _uiScalingSlider.value = value;
        }
        public void SetEnemySpeed(float value) => _settings.EncounterSettings.EnemySpeedMultiplier = value;
        public void SetUIScaling(float value) => _settings.UIScaling = value;
    }
}