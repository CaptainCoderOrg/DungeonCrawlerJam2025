using System.Collections;

using CaptainCoder.Dungeoneering.Encounter;
using CaptainCoder.Unity.Assertions;

using NaughtyAttributes;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace CaptainCoder.Dungeoneering.CrawlingMode
{
    public class DialogueController : MonoBehaviour
    {
        [AssertIsSet][SerializeField] private TextMeshProUGUI _topDialogue;
        [AssertIsSet][SerializeField] private TextMeshProUGUI _bottomDialouge;
        [AssertIsSet][SerializeField] private Image _portraitImage;
        [AssertIsSet][SerializeField] private ToggleablePanel _portrait;
        [AssertIsSet][SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _fadeDuration = 0.25f;
        [SerializeField] private float _messageDisplayDuration = 1f;
        private Coroutine _printMessageRoutine;

        [SerializeField] private string _testMessage;

        public Sprite Portrait
        {
            get => _portraitImage.sprite;
            internal set
            {
                _portraitImage.sprite = value;
                _portrait.IsEnabled = _portraitImage.sprite != null;
            }
        }

        [Button]
        private void TestMessage() => ShowMessage(_testMessage);

        [Button]
        public void Hide()
        {
            StartCoroutine(FadeTo(0));
        }

        public void ShowMessage(string message)
        {
            _topDialogue.maxVisibleCharacters = 0;
            _topDialogue.text = message;
            StartCoroutine(FadeTo(1));
            if (_printMessageRoutine != null) { StopCoroutine(_printMessageRoutine); }
            _printMessageRoutine = StartCoroutine(PrintMessage());
        }

        private IEnumerator PrintMessage()
        {
            float elapsed = 0;
            int length = _topDialogue.text.Length;
            while (elapsed < _messageDisplayDuration)
            {
                yield return null;
                elapsed += Time.deltaTime;
                float percent = elapsed / _messageDisplayDuration;
                _topDialogue.maxVisibleCharacters = (int)(length * percent);
                _bottomDialouge.maxVisibleCharacters = _topDialogue.maxVisibleCharacters;
            }
            _topDialogue.maxVisibleCharacters = length;
            _bottomDialouge.maxVisibleCharacters = length;
        }

        private IEnumerator FadeTo(float targetAlpha)
        {
            float startAlpha = _canvasGroup.alpha;
            float elapsed = 0;
            while (elapsed < _fadeDuration)
            {
                yield return null;
                elapsed += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / _fadeDuration);
            }
            _canvasGroup.alpha = targetAlpha;
        }

    }
}