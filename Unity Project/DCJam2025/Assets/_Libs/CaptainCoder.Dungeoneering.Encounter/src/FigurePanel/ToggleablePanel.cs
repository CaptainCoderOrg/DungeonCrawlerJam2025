using System.Collections;

using CaptainCoder.Unity.Assertions;

using UnityEngine;
using UnityEngine.UI;
namespace CaptainCoder.Dungeoneering.Encounter
{
    [RequireComponent(typeof(CanvasGroup), typeof(LayoutElement))]
    public class ToggleablePanel : MonoBehaviour
    {

        [AssertIsSet][SerializeField] private CanvasGroup _canvasGroup;
        [AssertIsSet][SerializeField] private LayoutElement _layoutElement;
        [SerializeField] private bool _isEnabled = true;
        [SerializeField] private bool _ignoreLayoutAlways = false;
        public event System.Action OnChange;
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                if (_isEnabled) { Show(); }
                else { Hide(); }
            }
        }

        void Awake()
        {
            _canvasGroup ??= GetComponent<CanvasGroup>();
            _layoutElement ??= GetComponent<LayoutElement>();
            if (_isEnabled) { Show(); }
            else { Hide(); }
        }

        public void Show()
        {
            StopAllCoroutines();
            _layoutElement.ignoreLayout = _ignoreLayoutAlways || false;
            OnChange?.Invoke();
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(ShowAtEndOfFrame());
            }
        }

        public IEnumerator ShowAtEndOfFrame()
        {
            yield return null;
            _canvasGroup.alpha = 1;
            _canvasGroup.blocksRaycasts = true;
            _layoutElement.ignoreLayout = _ignoreLayoutAlways || false;
        }

        public void Hide()
        {
            StopAllCoroutines();
            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;
            _layoutElement.ignoreLayout = _ignoreLayoutAlways || true;
            OnChange?.Invoke();
        }

        public void Toggle() => IsEnabled = !IsEnabled;
    }
}