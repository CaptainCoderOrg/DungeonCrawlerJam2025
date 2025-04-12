using CaptainCoder.Dungeoneering.Encounter;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.CrawlingMode
{
    [CreateAssetMenu(menuName = "DC/Events/Action/Start Encounter")]
    public class StartEncounterActionData : EventActionData
    {
        [field: SerializeField] public EncounterData Encounter { get; private set; }

        public override void Execute(CrawlerLogicController controller)
        {
            base.Execute(controller);
            controller.StartEncounter(Encounter);
        }
    }
}