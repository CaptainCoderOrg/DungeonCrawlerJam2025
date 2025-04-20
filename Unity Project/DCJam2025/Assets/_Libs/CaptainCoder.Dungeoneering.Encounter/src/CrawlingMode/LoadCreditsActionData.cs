using UnityEngine;
namespace CaptainCoder.Dungeoneering.CrawlingMode
{
    [CreateAssetMenu(menuName = "DC/Events/Action/Load Credits Action")]
    public class LoadCreditsActionData : EventActionData
    {
        public override void Execute(CrawlerLogicController logicController)
        {
            logicController.LoadCredits();
        }
    }
}