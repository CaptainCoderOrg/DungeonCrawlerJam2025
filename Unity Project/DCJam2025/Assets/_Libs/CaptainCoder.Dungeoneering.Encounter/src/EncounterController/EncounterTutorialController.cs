
using System;

using CaptainCoder.Unity.Assertions;

using NaughtyAttributes;

using UnityEngine;
using UnityEngine.EventSystems;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class EncounterTutorialController : MonoBehaviour, IPointerClickHandler
    {
        [AssertIsSet][SerializeField] private TutorialData _tutorialData;
        public TutorialData TutorialData => _tutorialData;
        [SerializeField] private TutorialEntry[] _entries;

        public event Action OnClick;

        [Button]
        private void InitializeEntries()
        {
            _entries = new TutorialEntry[14];
            _entries[0] = new() { Name = TutorialData._00TakeTurn };
            _entries[1] = new() { Name = TutorialData._01AttackAndMove };
            _entries[2] = new() { Name = TutorialData._02SelectMovement };
            _entries[3] = new() { Name = TutorialData._03MoveAdjacentToEnemy };
            _entries[4] = new() { Name = TutorialData._04SelectAttackPanel };
            _entries[5] = new() { Name = TutorialData._05SelectTarget };
            _entries[6] = new() { Name = TutorialData._06TargetInformation };
            _entries[7] = new() { Name = TutorialData._07Accuracy };
            _entries[8] = new() { Name = TutorialData._08MissingByAccuracy };
            _entries[9] = new() { Name = TutorialData._13RollingAMiss };
            _entries[10] = new() { Name = TutorialData._09Damage };
            _entries[11] = new() { Name = TutorialData._10BonusDie };
            _entries[12] = new() { Name = TutorialData._11Exert };
            _entries[13] = new() { Name = TutorialData._12AbilityPoints };
            _entries[14] = new() { Name = TutorialData._15OpenSettings };
        }

        private ToggleablePanel _currentlyVisible;

        public void HideTutorial()
        {
            if (_currentlyVisible == null) { return; }
            _currentlyVisible.IsEnabled = false;
        }

        private void ShowTutorial(string tutorial)
        {
            _tutorialData.AddTutorial(tutorial);
            foreach (var entry in _entries)
            {
                if (entry.Name == tutorial)
                {
                    entry.Panel.IsEnabled = true;
                    _currentlyVisible = entry.Panel;
                    if (entry.Panel.TryGetComponent<BlockingTutorial>(out var blocker))
                    {
                        blocker.Activate();
                    }
                }
                else
                {
                    entry.Panel.IsEnabled = false;
                }
            }
        }

        public void ShowTutorialIfNeverSeen(string tutorial)
        {
            if (_tutorialData.HasSeenTutorial(tutorial)) { return; }
            ShowTutorial(tutorial);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick?.Invoke();
        }

    }

    [Serializable]
    public struct TutorialEntry
    {
        public string Name;
        public ToggleablePanel Panel;
    }
}