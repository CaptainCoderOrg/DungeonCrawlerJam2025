using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    [CreateAssetMenu(menuName = "DC/EncounterSettingsData")]
    public class EncounterSettingsData : ObservableSO
    {
        public float TargetZoom = 5;
        public float TargetRotation = 0;
        public int TargetPitch = 1;
        [SerializeField] private float _enemyDelay = 1f;
        public float EnemyDelayTime
        {
            get => _enemyDelay;
            set
            {
                _enemyDelay = value;
                EnemyDelay = new WaitForSecondsRealtime(_enemyDelay);
            }
        }
        public WaitForSecondsRealtime EnemyDelay { get; private set; }
        [SerializeField] private float _movementSpeed = 0.1f;
        public float MovementSpeed
        {
            get => _movementSpeed;
            set
            {
                _movementSpeed = value;
                WaitForMovement = new WaitForSeconds(_movementSpeed);
            }
        }
        public WaitForSeconds WaitForMovement { get; private set; }

        void OnValidate()
        {
            WaitForMovement = new WaitForSeconds(_movementSpeed);
            EnemyDelay = new WaitForSecondsRealtime(_enemyDelay);
        }

        public override void OnAfterEnterPlayMode()
        {
            base.OnAfterEnterPlayMode();
            WaitForMovement = new WaitForSeconds(MovementSpeed);
            EnemyDelay = new WaitForSecondsRealtime(_enemyDelay);
            FinishedEncounter = null;
        }

        protected override void OnExitPlayMode()
        {
            base.OnExitPlayMode();
            FinishedEncounter = null;
        }

        public EncounterData TargetEncounter;
        public EncounterData FinishedEncounter;
    }
}