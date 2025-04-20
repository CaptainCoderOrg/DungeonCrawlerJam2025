using System.Collections;

using CaptainCoder.Unity.Assertions;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Unity
{
    public class ScreenHider : MonoBehaviour
    {
        [SerializeField] private float _duration = 1;
        [AssertIsSet][SerializeField] private CanvasGroup _canvasGroup;

        public event System.Action OnFinished;

        public void Hide()
        {
            StopAllCoroutines();
            StartCoroutine(FadeTo(1f));
        }

        private IEnumerator FadeTo(float targetAlpha)
        {
            float startAlpha = _canvasGroup.alpha;
            float elapsed = 0;
            while (elapsed < _duration)
            {
                yield return null;
                elapsed += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / _duration);
            }
            _canvasGroup.alpha = targetAlpha;
            OnFinished?.Invoke();
        }

        public void Show()
        {
            StopAllCoroutines();
            StartCoroutine(FadeTo(0f));
        }


        public IEnumerator HideScreenCoroutine()
        {
            yield return StartCoroutine(FadeTo(1f));
        }
    }
}
