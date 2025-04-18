using CaptainCoder.Dungeoneering.Encounter;
using CaptainCoder.Unity.Assertions;

using TMPro;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CaptainCoder.Dungeoneering.CrawlingMode
{

    public class OptionButton : MonoBehaviour, IPointerClickHandler
    {

        [AssertIsSet][SerializeField] private TextMeshProUGUI _textLabel;
        [AssertIsSet][SerializeField] private ToggleablePanel _toggleablePanel;
        private Color _resetColor;
        [SerializeField] private Image _buttonImage;
        public Color Color
        {
            get => _buttonImage.color;
            set => _buttonImage.color = value;
        }
        [field: SerializeField] public UnityEvent OnClick { get; private set; }

        void Awake()
        {
            _buttonImage = GetComponent<Image>();
            _resetColor = _buttonImage.color;
        }

        public void ResetColor()
        {
            Color = _resetColor;
        }

        public string Text
        {
            get => _textLabel.text;
            set
            {
                _textLabel.text = value;
            }
        }

        public bool IsVisible
        {
            get => _toggleablePanel.IsEnabled;
            set => _toggleablePanel.IsEnabled = value;
        }

        public void OnPointerClick(PointerEventData eventData) => OnClick?.Invoke();
    }
}