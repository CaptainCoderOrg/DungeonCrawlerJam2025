using UnityEngine;
namespace CaptainCoder.Dungeoneering.CrawlingMode
{

    public class LightFlickerController : MonoBehaviour
    {
        [SerializeField] private float _minIntensity;
        [SerializeField] private float _maxIntensity;
        [SerializeField] private float _flickerSpeed;
        [SerializeField] private Light _light;

        void Update()
        {
            _light.intensity = _minIntensity + ((Mathf.Sin(Time.time * _flickerSpeed) + 1) * 0.5f) * (_maxIntensity - _minIntensity);
        }
    }
}