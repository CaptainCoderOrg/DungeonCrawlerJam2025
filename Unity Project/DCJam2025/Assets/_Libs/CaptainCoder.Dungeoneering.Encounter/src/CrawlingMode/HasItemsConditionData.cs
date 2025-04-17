
using System.Collections.Generic;
using System.Linq;

using CaptainCoder.Dungeoneering.Encounter;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.CrawlingMode
{
    [CreateAssetMenu(menuName = "DC/Events/Conditions/Has Items")]
    public class HasItemConditionData : EventConditionData
    {
        [SerializeField] private HeroEntityData[] _heroes;
        [SerializeField] private ContainerData _playerInventory;
        [SerializeField] private EquipmentData[] _equipment;
        public override bool ConditionMet()
        {
            HashSet<EquipmentData> allItems = _heroes.SelectMany(h => h.AllItems).Concat(_playerInventory.EquipmentData).ToHashSet();
            foreach (EquipmentData equipment in _equipment)
            {
                if (!allItems.Contains(equipment)) { return false; }
            }
            return true;
        }
}
}