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

    public class WarningModalController : MonoBehaviour
    {
        [AssertIsSet][SerializeField] private ToggleablePanel _toggleablePanel;
        [AssertIsSet][SerializeField] private TextMeshProUGUI _messageLabel;
        [AssertIsSet][SerializeField] private RectTransform _panel;
        private Action _onConfirm;

        public void ShowWarning(string message, Action onConfirm)
        {
            _messageLabel.text = message;
            _toggleablePanel.Show();
            _onConfirm = onConfirm;
            Rebuild();
        }

        public void Confirm()
        {
            _toggleablePanel.Hide();
            _onConfirm.Invoke();
        }

        [Button]
        private void Rebuild()
        {
            StartCoroutine(RebuildAtEndOfFrame());
        }

        private IEnumerator RebuildAtEndOfFrame()
        {
            yield return null;
            LayoutRebuilder.ForceRebuildLayoutImmediate(_panel);
        }
    }
}