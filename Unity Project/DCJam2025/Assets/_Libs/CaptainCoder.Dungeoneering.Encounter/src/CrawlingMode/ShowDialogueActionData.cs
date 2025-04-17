using System;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.CrawlingMode
{
    [CreateAssetMenu(menuName = "DC/Events/Action/Show Dialogue Action")]
    public class ShowDialogueActionData : EventActionData
    {
        [field: SerializeField] public DialogueEntry Entry { get; private set; }
        [field: SerializeField] public DialogueOption[] Options { get; private set; }
        public EventActionData OnSkip { get; private set; }

        public override void Execute(CrawlerLogicController logicController)
        {
            logicController.ShowDialogue(Entry.Portrait, Entry.Message, Options);
        }
    }

    [Serializable]
    public struct DialogueOption
    {
        public string Text;
        public EventActionData[] OnSelected;
    }
}