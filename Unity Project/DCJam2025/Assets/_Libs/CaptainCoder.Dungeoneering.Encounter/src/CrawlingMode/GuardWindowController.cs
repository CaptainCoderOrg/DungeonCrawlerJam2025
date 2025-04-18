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
        [AssertIsSet][SerializeField] private DiceHUD _diceHUD;
        [AssertIsSet][SerializeField] private TileHighlighter _tileHighlighter;
        [AssertIsSet][SerializeField] private ToggleablePanel _toggleablePanel;
        [AssertIsSet][SerializeField] private TextMeshProUGUI _messageLabel;
        [AssertIsSet][SerializeField] private RectTransform _panel;
        [AssertIsSet][SerializeField] private OptionButton[] _optionButtons;
        [AssertIsSet][SerializeField] private OptionButton _confirmGuard;
        private bool _isFinished;
        private WaitUntil _waitUntilFinished;
        private WaitUntil WaitUntilFinished => _waitUntilFinished ??= new WaitUntil(IsFinished);
        private GuardInfo _selectedGuard;
        private OptionButton _selectedButton;
        private readonly HashSet<GuardInfo> _possibleGuards = new();

        public void ShowWindow(string message, IEnumerable<GuardInfo> possibleGuards)
        {
            _possibleGuards.Clear();
            _possibleGuards.UnionWith(possibleGuards);
            _messageLabel.text = message;
            UpdateButtons();
            _toggleablePanel.Show();
            Rebuild();
        }

        private void UpdateButtons()
        {
            _selectedGuard = null;
            _confirmGuard.IsVisible = false;
            int ix = 0;
            foreach (GuardInfo guard in _possibleGuards)
            {
                OptionButton button = _optionButtons[ix++];
                button.ResetColor();
                button.IsVisible = true;
                button.Text = $"Guard with {guard.Hero.FigureController.Figure.EntityData.ShortName}";
                button.OnClick.RemoveAllListeners();
                button.OnClick.AddListener(() => HandleSelectGuard(guard, button));
            }
            for (; ix < _optionButtons.Length; ix++)
            {
                OptionButton button = _optionButtons[ix];
                button.IsVisible = false;
                button.OnClick.RemoveAllListeners();
            }
        }

        private void HandleSelectGuard(GuardInfo guardInfo, OptionButton selectedButton)
        {
            Clear();
            _selectedButton = selectedButton;
            selectedButton.Color = Color.gray;
            _selectedGuard = guardInfo;
            Debug.Log(guardInfo.Hero.FigureController.Figure.EntityData.Name);
            _tileHighlighter.Guard(guardInfo.AttackInfo);
            _confirmGuard.IsVisible = true;
            _confirmGuard.OnClick.RemoveAllListeners();
            _confirmGuard.OnClick.AddListener(PerformAttack);
            Rebuild();
            // AttackData attack = ((HeroEntityData)hero.FigureController.Figure.EntityData).PossibleAttacks().First();
            // hero.FigureController.Figure.FindAttackTargets(attack, state, data).Where(info => info.TargetPosition);
        }

        private void Clear()
        {
            if (_selectedGuard != null)
            {
                _tileHighlighter.Selected(_selectedGuard.AttackInfo.TargetPosition);
            }
            if (_selectedButton != null)
            {
                _selectedButton.ResetColor();
            }
        }

        private void PerformAttack()
        {
            _selectedGuard.Hero.FigureController.Figure.EntityData.RemoveEffectsWhere(LivingEntityData.IsGuardEffect);
            _possibleGuards.Remove(_selectedGuard);
            AttackInfo attack = _selectedGuard.AttackInfo;
            Debug.Log($"Attack Picked: {attack}");
            if (attack == null) { return; }
            StartCoroutine(PerformAttack(attack, _selectedGuard.AttackData, _selectedGuard.Attacker(), _selectedGuard.AttackDice()));
        }

        private IEnumerator PerformAttack(AttackInfo attackInfo, AttackData attackData, FigureData attacker, IEnumerable<DieData> attackDice)
        {
            _diceHUD.AttackInfo = attackInfo;
            _diceHUD.Attack = attackData;
            _diceHUD.Attacker = attacker;
            _diceHUD.SetDice(attackDice);
            _diceHUD.Show();
            _toggleablePanel.Hide();
            yield return StartCoroutine(_diceHUD.Roll());
            yield return _diceHUD.WaitForConfirm();
            // TODO: Check for death
            UpdateButtons();
            _toggleablePanel.Show();
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
            Clear();
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