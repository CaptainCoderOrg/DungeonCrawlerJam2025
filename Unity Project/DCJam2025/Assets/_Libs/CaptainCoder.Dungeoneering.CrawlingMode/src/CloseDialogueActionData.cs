using UnityEngine;
namespace CaptainCoder.Dungeoneering.CrawlingMode
{
    [CreateAssetMenu(menuName = "DC/Events/Action/Close Dialogue Action")]
    public class CloseDialogueActionDAta : EventActionData
    {
        public override void Execute(CrawlerLogicController logicController)
        {
            logicController.HideDialogue();
        }
    }
}