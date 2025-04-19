using System;

using CaptainCoder.Dungeoneering.Encounter;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.CrawlingMode
{
    [CreateAssetMenu(menuName = "DC/Events/Action/Encounter Dialogue Action")]
    public class EncounterDialogueActionData : EventActionData
    {
        [field: SerializeField] public EncounterData Encounter { get; private set; }
        [field: SerializeField] public DialogueEntry Entry { get; private set; }
        [field: SerializeField] public DialogueOption[] Options { get; private set; }
        public EventActionData OnSkip { get; private set; }

        public override void Execute(CrawlerLogicController logicController)
        {
            if (Encounter.IsComplete) { return; }
            logicController.ShowDialogue(Entry.Portrait, Entry.Message, Options);
        }
    }
}