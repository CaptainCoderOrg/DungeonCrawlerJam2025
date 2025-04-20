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
        private CrawlerLogicController _crawlerLogicController;
        [AssertIsSet][SerializeField] private OptionsSettings _settings;
        [AssertIsSet][SerializeField] private ToggleablePanel _toggleablePanel;
        [AssertIsSet][SerializeField] private TextMeshProUGUI _uiScalingLabel;
        [AssertIsSet][SerializeField] private Slider _uiScalingSlider;
        [AssertIsSet][SerializeField] private TextMeshProUGUI _enemySpeedLabel;
        [AssertIsSet][SerializeField] private Slider _enemySpeedSlider;
        [AssertIsSet][SerializeField] private Toggle _autoConfirmToggle;
        [AssertIsSet][SerializeField] private TextMeshProUGUI _masterVolumeLabel;
        [AssertIsSet][SerializeField] private Slider _masterVolumeSlider;
        [AssertIsSet][SerializeField] private TextMeshProUGUI _musicVolumeLabel;
        [AssertIsSet][SerializeField] private Slider _musicVolumeSlide;
        [AssertIsSet][SerializeField] private TextMeshProUGUI _soundVolumeLabel;
        [AssertIsSet][SerializeField] private Slider _soundVolumeSlider;
        [AssertIsSet][SerializeField] private AudioSource _soundCheck;

        void Start()
        {
            _crawlerLogicController = FindFirstObjectByType<CrawlerLogicController>();
            _settings.OnUIScalingChanged += HandleUIScalingChanged;
            _settings.EncounterSettings.OnEnemySpeedChanged += HandleEnemySpeedChange;
            _settings.EncounterSettings.OnAutoConfirmChanged += HandleConfirmChanged;
            HandleUIScalingChanged(_settings.UIScaling);
            HandleEnemySpeedChange(_settings.EncounterSettings.EnemySpeedMultiplier);
            HandleConfirmChanged(_settings.EncounterSettings.AutoConfirmEnemy);
            _musicVolumeSlide.onValueChanged.AddListener((value) => HandleVolumeChange(value, "Music", _musicVolumeLabel));
            HandleVolumeChange(_musicVolumeSlide.value, "Music", _musicVolumeLabel);
            _masterVolumeSlider.onValueChanged.AddListener((value) => HandleVolumeChange(value, "Master Volume", _masterVolumeLabel));
            HandleVolumeChange(_masterVolumeSlider.value, "Master Volume", _masterVolumeLabel);
            _soundVolumeSlider.onValueChanged.AddListener((value) => HandleVolumeChange(value, "SFX", _soundVolumeLabel));
            HandleVolumeChange(_soundVolumeSlider.value, "SFX", _soundVolumeLabel);
            if (!_settings.HasBeenOpen)
            {
                _toggleablePanel.IsEnabled = true;
                _settings.HasBeenOpen = true;
            }
        }

        private void HandleVolumeChange(float value, string prefix, TextMeshProUGUI musicVolumeLabel)
        {
            if (prefix == "SFX" && Time.timeSinceLevelLoad > 0.1 && !_soundCheck.isPlaying)
            {
                _soundCheck.Play();
            }
            musicVolumeLabel.text = $"{prefix}: {(int)(value * 100)}%";
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

        public void Panic()
        {
            _crawlerLogicController?.Panic();
        }
    }
}