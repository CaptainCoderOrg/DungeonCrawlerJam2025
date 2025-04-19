using System.Collections.Generic;

using NaughtyAttributes;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    [CreateAssetMenu(menuName = "DC/Tutorial Data")]
    public sealed class TutorialData : ObservableSO
    {
        public const string _00TakeTurn = "TakeTurn";
        public const string _01AttackAndMove = "AttackAndMove";
        public const string _02SelectMovement = "SelectMovement";
        public const string _03MoveAdjacentToEnemy = "MoveAdjacentToEnemy";
        public const string _04SelectAttackPanel = "SelectAttackPanel";
        public const string _05SelectTarget = "SelectTarget";
        public const string _06TargetInformation = "TargetInformation";
        public const string _07Accuracy = "Accuracy";
        public const string _08MissingByAccuracy = "MisisngByAccuracy";
        public const string _13RollingAMiss = "RollingAMiss";
        public const string _09Damage = "Damage";
        public const string _10BonusDie = "BonusDie";
        public const string _11Exert = "Exert";
        public const string _12AbilityPoints = "AbilityPoints";
        public const string _14EndTurn = "EndTurn";


        [SerializeField] private bool _resetOnStart = true;

        [field: SerializeField] public bool IsDisabled { get; private set; } = false;
        [field: SerializeField] public List<string> TutorialsSeen { get; private set; }

        public bool HasSeenTutorial(string name) => IsDisabled || TutorialsSeen.Contains(name);
        public void AddTutorial(string name) => TutorialsSeen.Add(name);

        [Button]
        public void ResetTutorials()
        {
            TutorialsSeen.Clear();
        }

        public override void OnAfterEnterPlayMode()
        {
            base.OnAfterEnterPlayMode();
            if (_resetOnStart) { ResetTutorials(); }
        }
    }
}