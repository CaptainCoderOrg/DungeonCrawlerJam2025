using CaptainCoder.Dungeoneering.Encounter;
using CaptainCoder.Unity.Assertions;

using TMPro;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace CaptainCoder.Dungeoneering.CrawlingMode
{

    public class OptionButton : MonoBehaviour, IPointerClickHandler
    {
        [AssertIsSet][SerializeField] private TextMeshProUGUI _textLabel;
        [AssertIsSet][SerializeField] private ToggleablePanel _toggleablePanel;
        [field: SerializeField] public UnityEvent OnClick { get; private set; }

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