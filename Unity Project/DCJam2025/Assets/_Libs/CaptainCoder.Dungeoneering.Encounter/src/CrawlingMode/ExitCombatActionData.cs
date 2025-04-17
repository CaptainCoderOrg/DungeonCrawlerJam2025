using UnityEngine;
namespace CaptainCoder.Dungeoneering.CrawlingMode
{
    [CreateAssetMenu(menuName = "DC/Events/Action/ExitEncounterActionData")]
    public class ExitEncounterActionData : EventActionData
    {
        public override void Execute(CrawlerLogicController logicController)
        {
            logicController.EndEncounter();
        }
    }
}