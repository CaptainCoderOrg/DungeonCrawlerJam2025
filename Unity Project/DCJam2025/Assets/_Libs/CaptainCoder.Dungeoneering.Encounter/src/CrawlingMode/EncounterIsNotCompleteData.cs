using CaptainCoder.Dungeoneering.Encounter;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.CrawlingMode
{
    [CreateAssetMenu(menuName = "DC/Events/Conditions/Encounter Is Not Complete")]
    public class EncounterIsNotCompleteData : EventConditionData
    {
        [SerializeField] private EncounterData _encounter;
        public override bool ConditionMet() => !_encounter.IsComplete;
    }
}