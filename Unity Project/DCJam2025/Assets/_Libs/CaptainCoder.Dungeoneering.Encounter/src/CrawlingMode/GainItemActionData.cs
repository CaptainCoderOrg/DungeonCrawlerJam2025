using CaptainCoder.Dungeoneering.Encounter;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.CrawlingMode
{
    [CreateAssetMenu(menuName = "DC/Events/Action/Gain Item Action")]
    public class GainItemActionData : EventActionData
    {
        [SerializeField] private ContainerData _playerInventory;
        [SerializeField] private EquipmentData _equipment;
        public override void Execute(CrawlerLogicController logicController)
        {
            logicController.AnimateReceivingItem(_equipment);
            foreach (var slot in _playerInventory.EquipmentSlots)
            {
                if (slot.Data == null)
                {
                    slot.Data = _equipment;
                    return;
                }
            }
            Debug.LogWarning("TODO: Implement giving item when inventory is full");
        }
    }
}