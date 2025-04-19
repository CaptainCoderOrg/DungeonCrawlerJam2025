using System.Collections.Generic;
using System.Linq;

using CaptainCoder.Dungeoneering.Encounter;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.CrawlingMode
{
    [CreateAssetMenu(menuName = "DC/Loot Table Data")]
    public class LootTableData : ObservableSO
    {
        public EquipmentData[] PossibleItems;
        public LootTableEntry[] MultipleItems;
        private readonly Queue<EquipmentData> _itemQueue = new();
        public EquipmentData GetRandomItem() => NextItem();

        private EquipmentData NextItem()
        {
            if (_itemQueue.Count == 0)
            {
                IEnumerable<EquipmentData> shuffled = MultipleItems.SelectMany(e => Enumerable.Repeat(e.Item, e.Count)).Concat(PossibleItems).OrderBy(_ => Random.Range(0f, 1f));
                foreach (var item in shuffled)
                {
                    _itemQueue.Enqueue(item);
                }
            }
            return _itemQueue.Dequeue();
        }

        public override void OnBeforeEnterPlayMode()
        {
            base.OnBeforeEnterPlayMode();
            _itemQueue.Clear();
        }
    }

    [System.Serializable]
    public struct LootTableEntry
    {
        public EquipmentData Item;
        public int Count;
    }
}