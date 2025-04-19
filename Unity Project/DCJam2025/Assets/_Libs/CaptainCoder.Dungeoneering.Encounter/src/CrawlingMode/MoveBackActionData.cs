using UnityEngine;
namespace CaptainCoder.Dungeoneering.CrawlingMode
{
    [CreateAssetMenu(menuName = "DC/Events/Action/Move Back Action")]
    public class MoveBackActionData : EventActionData
    {
        public override void Execute(CrawlerLogicController logicController)
        {
            logicController.MoveBack();
        }
    }
}