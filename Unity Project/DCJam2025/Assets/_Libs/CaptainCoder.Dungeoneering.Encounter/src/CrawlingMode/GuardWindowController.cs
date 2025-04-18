using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using CaptainCoder.Dungeoneering.Encounter;
using CaptainCoder.Unity.Assertions;

using NaughtyAttributes;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace CaptainCoder.Dungeoneering.CrawlingMode
{

    public class GuardWindowController : MonoBehaviour
    {
        [AssertIsSet][SerializeField] private ToggleablePanel _toggleablePanel;
        [AssertIsSet][SerializeField] private TextMeshProUGUI _messageLabel;
        [AssertIsSet][SerializeField] private RectTransform _panel;
        [AssertIsSet][SerializeField] private OptionButton[] _optionButtons;
        private bool _isFinished;
        private WaitUntil _waitUntilFinished;
        private WaitUntil WaitUntilFinished => _waitUntilFinished ??= new WaitUntil(IsFinished);

        public void ShowWindow(string message, IEnumerable<GuardInfo> possibleGuards)
        {
            _messageLabel.text = message;
            int ix = 0;
            foreach (GuardInfo guard in possibleGuards)
            {
                OptionButton button = _optionButtons[ix++];
                button.IsVisible = true;
                button.Text = $"Guard with {guard.Hero.FigureController.Figure.EntityData.ShortName}";
                button.OnClick.RemoveAllListeners();
                button.OnClick.AddListener(() => HandleSelectGuard(guard));
            }
            for (; ix < _optionButtons.Length; ix++)
            {
                OptionButton button = _optionButtons[ix];
                button.IsVisible = false;
                button.OnClick.RemoveAllListeners();
            }
            _toggleablePanel.Show();
            Rebuild();
        }

        private void HandleSelectGuard(GuardInfo guardInfo)
        {
            Debug.Log(guardInfo.Hero.FigureController.Figure.EntityData.Name);
            // AttackData attack = ((HeroEntityData)hero.FigureController.Figure.EntityData).PossibleAttacks().First();
            // hero.FigureController.Figure.FindAttackTargets(attack, state, data).Where(info => info.TargetPosition);
        }

        public IEnumerator ShowGuardWindowAndWait(IEnumerable<GuardInfo> possibleGuards)
        {
            if (!possibleGuards.Any()) { yield break; }
            ShowWindow($"{possibleGuards.First().Enemy.Figure.EntityData.Name} is about to move. Would you like to guard?", possibleGuards);
            _isFinished = false;
            yield return WaitUntilFinished;
        }

        private bool IsFinished() => _isFinished;

        public void EndGuarding()
        {
            _isFinished = true;
            _toggleablePanel.Hide();
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