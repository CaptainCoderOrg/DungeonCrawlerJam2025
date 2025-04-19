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
        private readonly Queue<EquipmentData> _itemQueue = new();
        public EquipmentData GetRandomItem() => NextItem();

        private EquipmentData NextItem()
        {
            if (_itemQueue.Count == 0)
            {
                foreach (var item in PossibleItems.OrderBy(_ => Random.Range(0f, 1f)))
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
}