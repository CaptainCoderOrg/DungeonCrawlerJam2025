using CaptainCoder.Dungeoneering.Encounter;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.CrawlingMode
{
    [CreateAssetMenu(menuName = "DC/Events/Action/Random Loot Action Data")]
    public class RandomLootActionData : EventActionData
    {
        public int MinItems = 1;
        public int MaxItems = 2;
        public EquipmentData[] PossibleItems;
        public DialogueEntry Dialogue;
        public DialogueOption[] Options;
        public override void Execute(CrawlerLogicController logicController)
        {
            EquipmentData randomItem = PossibleItems[Random.Range(0, PossibleItems.Length)];
            logicController.ShowDialogue(Dialogue.Portrait, $"Searching the carnage you find a {randomItem.Name}", Options);
            logicController.GainItem(randomItem);
        }
    }
}