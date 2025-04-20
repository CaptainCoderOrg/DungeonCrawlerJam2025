
using System.Collections.Generic;
using System.Linq;

using CaptainCoder.Dungeoneering.Encounter;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.CrawlingMode
{
    [CreateAssetMenu(menuName = "DC/Events/Conditions/Does Not Have Items")]
    public class DoesNotHaveItemsDonditionData : EventConditionData
    {
        [SerializeField] private HeroEntityData[] _heroes;
        [SerializeField] private ContainerData _heroInventory;
        [SerializeField] private EquipmentData[] _equipment;
        public override bool ConditionMet()
        {
            HashSet<EquipmentData> allItems = _equipment.ToHashSet();
            foreach (EquipmentData equipment in _heroes.SelectMany(h => h.AllItems).Concat(_heroInventory.EquipmentData).ToHashSet())
            {
                allItems.Remove(equipment);
            }
            return allItems.Count > 0;
        }
    }
}