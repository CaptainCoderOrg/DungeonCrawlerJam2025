using NaughtyAttributes;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.CrawlingMode
{
    [CreateAssetMenu(menuName = "DC/Events/Action/Show Dialogue Action")]
    public class ShowDialogueActionData : EventActionData
    {
        [field: TextArea(3, 5)][field: SerializeField] public string Message { get; private set; }
        [field: ShowAssetPreview][field: SerializeField] public Sprite Portrait { get; private set; }

        public override void Execute(CrawlerLogicController logicController)
        {
            logicController.ShowDialogue(Portrait, Message);
        }
    }
}