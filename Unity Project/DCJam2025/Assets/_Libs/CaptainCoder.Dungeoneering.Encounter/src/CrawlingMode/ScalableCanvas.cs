using CaptainCoder.Dungeoneering.CrawlingMode;
using CaptainCoder.Unity.Assertions;


using UnityEngine;
using UnityEngine.UI;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class ScalableCanvas : MonoBehaviour
    {
        [AssertIsSet][SerializeField] private OptionsSettings _settings;
        [AssertIsSet][SerializeField] private CanvasScaler _canvas;

        void OnEnable()
        {
            if (!_settings.HasInitialized)
            {
                _settings.Initialize();
            }
            _settings.OnUIScalingChanged += HandleUIScalingChanged;
            _canvas.scaleFactor = _settings.UIScaling;
        }

        void OnDisable()
        {
            _settings.OnUIScalingChanged -= HandleUIScalingChanged;
        }

        private void HandleUIScalingChanged(float value)
        {
            _canvas.scaleFactor = value;
        }
    }
}