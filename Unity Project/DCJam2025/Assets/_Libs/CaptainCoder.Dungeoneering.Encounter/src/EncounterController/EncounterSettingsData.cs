using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    [CreateAssetMenu(menuName = "DC/EncounterSettingsData")]
    public class EncounterSettingsData : ObservableSO
    {

        [SerializeField] private bool _autoConfirmEnemy;
        public bool AutoConfirmEnemy
        {
            get => _autoConfirmEnemy;
            set
            {
                _autoConfirmEnemy = value;
                OnAutoConfirmChanged?.Invoke(_autoConfirmEnemy);
            }
        }
        public event System.Action<bool> OnAutoConfirmChanged;
        public float TargetZoom = 5;
        public float TargetRotation = 0;
        public int TargetPitch = 1;
        [SerializeField] private float _enemySpeedMultiplier = 1;
        public float EnemySpeedMultiplier
        {
            get => _enemySpeedMultiplier;
            set
            {
                _enemySpeedMultiplier = value;
                EnemyDelay = new WaitForSecondsRealtime(_enemyDelay / _enemySpeedMultiplier);
                OnEnemySpeedChanged?.Invoke(_enemySpeedMultiplier);
            }
        }
        public event System.Action<float> OnEnemySpeedChanged;
        [SerializeField] private float _enemyDelay = 1f;
        public float EnemyDelayTime
        {
            get => _enemyDelay;
            set
            {
                _enemyDelay = value;
                EnemyDelay = RecalculateDelay();
            }
        }
        public WaitForSecondsRealtime EnemyDelay { get; private set; }
        private WaitForSecondsRealtime RecalculateDelay() => new (_enemyDelay / _enemySpeedMultiplier);
        [SerializeField] private float _movementSpeed = 0.1f;
        public float MovementSpeed
        {
            get => _movementSpeed;
            set
            {
                _movementSpeed = value;
                WaitForMovement = RecalculateDelay();
            }
        }
        public WaitForSecondsRealtime WaitForMovement { get; private set; }

        void OnValidate()
        {
            WaitForMovement = RecalculateDelay();
            EnemyDelay = new WaitForSecondsRealtime(_enemyDelay / _enemySpeedMultiplier);
        }

        public override void OnAfterEnterPlayMode()
        {
            base.OnAfterEnterPlayMode();
            WaitForMovement = RecalculateDelay();
            EnemyDelay = new WaitForSecondsRealtime(_enemyDelay / _enemySpeedMultiplier);
            FinishedEncounter = null;
        }

        protected override void OnExitPlayMode()
        {
            base.OnExitPlayMode();
            FinishedEncounter = null;
            OnEnemySpeedChanged = null;
            OnAutoConfirmChanged = null;
        }

        public EncounterData TargetEncounter;
        public EncounterData FinishedEncounter;
    }
}