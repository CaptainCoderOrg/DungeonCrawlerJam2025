using CaptainCoder.Dungeoneering.Encounter;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.CrawlingMode
{
    [CreateAssetMenu(menuName = "DC/Events/Action/Clear Inventory Action Data")]
    public class ClearInventoryActionData : EventActionData
    {
        [SerializeField] private ContainerData _playerInventory;
        public override void Execute(CrawlerLogicController logicController)
        {
            foreach (var slot in _playerInventory.EquipmentSlots)
            {
                slot.Data = null;
            }
        }
    }
}