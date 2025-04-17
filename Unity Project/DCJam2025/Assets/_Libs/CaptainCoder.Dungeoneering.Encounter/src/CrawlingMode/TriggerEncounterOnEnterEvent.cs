
using CaptainCoder.Dungeoneering.Encounter;

using NaughtyAttributes;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.CrawlingMode
{
    [CreateAssetMenu(menuName = "DC/Events/Trigger Encounter On Enter")]
    public class TriggerEncounterOnEnterEvent : CrawlerEnterTileEventData
    {
        [field: Expandable][field: SerializeField] public EncounterData Encounter { get; private set; }


        public override void OnAfterEnterPlayMode()
        {
            base.OnAfterEnterPlayMode();
            Actions.Clear();
            StartEncounterActionData encounterAction = CreateInstance<StartEncounterActionData>();
            encounterAction.Encounter = Encounter;
            Actions.Add(encounterAction);
        }

    }
}