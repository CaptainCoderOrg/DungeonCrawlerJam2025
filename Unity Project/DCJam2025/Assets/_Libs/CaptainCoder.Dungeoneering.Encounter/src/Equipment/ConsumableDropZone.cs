using CaptainCoder.Dungeoneering.Unity;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public sealed class ConsumableDropZone : MonoBehaviour
    {
        [SerializeField] private PartyData _partyData;
        [field: SerializeField] public HeroEntityData HeroEntityData { get; set; }


        public bool ConsumeItem(EquipmentData equipmentData)
        {
            if (equipmentData == null) { return false; }
            if (equipmentData is ConsumableEquipmentData consumable)
            {
                consumable.OnConsumed(HeroEntityData);
                return true;
            }
            return false;
        }
    }
}