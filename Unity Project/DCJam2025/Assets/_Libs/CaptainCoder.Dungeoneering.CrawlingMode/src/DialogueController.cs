using System;
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
        [AssertIsSet][SerializeField] private CrawlerLogicController _logicController;
        [AssertIsSet][SerializeField] private TextMeshProUGUI _topDialogue;
        [AssertIsSet][SerializeField] private TextMeshProUGUI _bottomDialouge;
        [AssertIsSet][SerializeField] private Image _portraitImage;
        [AssertIsSet][SerializeField] private ToggleablePanel _portrait;
        [AssertIsSet][SerializeField] private CanvasGroup _canvasGroup;
        [AssertIsSet][SerializeField] private OptionButton[] _optionButtons;
        [AssertIsSet][SerializeField] private OptionButton _skipButton;

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
            StartCoroutine(RebuildAtEndOfFrame());
            StartCoroutine(FadeTo(1));
            if (_printMessageRoutine != null) { StopCoroutine(_printMessageRoutine); }
            _printMessageRoutine = StartCoroutine(PrintMessage());
        }

        [Button]
        private void Rebuild()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
        }

        private IEnumerator RebuildAtEndOfFrame()
        {
            yield return null;
            Rebuild();
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

        private ChainedDialogueActionData _chainedDialogueActionData;
        private int _currentChainedIx;

        internal void ShowChainedDialogue(ChainedDialogueActionData chainedDialogueActionData)
        {
            _chainedDialogueActionData = chainedDialogueActionData;
            _currentChainedIx = -1;



            _optionButtons[0].IsVisible = true;
            _optionButtons[0].Text = "Next";
            _optionButtons[0].OnClick.RemoveAllListeners();
            _optionButtons[0].OnClick.AddListener(NextChained);

            _optionButtons[1].IsVisible = false;
            _optionButtons[1].Text = "Close";
            _optionButtons[1].OnClick.RemoveAllListeners();
            _optionButtons[1].OnClick.AddListener(FinishChainedDialogue);

            _optionButtons[2].Text = "Back";
            _optionButtons[2].IsVisible = false;
            _optionButtons[2].OnClick.RemoveAllListeners();
            _optionButtons[2].OnClick.AddListener(PrevChained);

            _skipButton.IsVisible = true;
            _skipButton.OnClick.RemoveAllListeners();
            _skipButton.OnClick.AddListener(FinishChainedDialogue);
            NextChained();
        }

        public void PrevChained() => NextChained(-1);
        public void NextChained() => NextChained(1);

        public void NextChained(int delta)
        {
            _currentChainedIx += delta;
            Portrait = _chainedDialogueActionData.Entries[_currentChainedIx].Portrait;
            ShowMessage(_chainedDialogueActionData.Entries[_currentChainedIx].Message);
            _optionButtons[0].IsVisible = _currentChainedIx < _chainedDialogueActionData.Entries.Length - 1;
            _optionButtons[1].IsVisible = _currentChainedIx == _chainedDialogueActionData.Entries.Length - 1;
            _optionButtons[2].IsVisible = _currentChainedIx > 0;
        }

        public void FinishChainedDialogue()
        {
            Hide();
            _chainedDialogueActionData?.OnFinished.Execute(_logicController);
        }
    }
}