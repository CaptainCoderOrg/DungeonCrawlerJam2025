using CaptainCoder.Dungeoneering.Unity;
using CaptainCoder.Unity.Assertions;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public sealed class EquipmentScrapDropZone : MonoBehaviour
    {

        [SerializeField] private PartyData _partyData;
        [AssertIsSet][SerializeField] private SoundDatabase _soundDatabase;

        public bool ScrapItem(EquipmentData equipmentData)
        {
            if (equipmentData == null) { return false; }
            if (equipmentData.IsKeyItem)
            {
                Debug.LogWarning($"TODO: Show warning message on key item destruction");
                return false;
            }
            _partyData.Gold += equipmentData.Value;
            _soundDatabase.Play("scrap");
            return true;
        }
    }
}