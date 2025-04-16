using System.Collections.Generic;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.CrawlingMode
{
    [CreateAssetMenu(menuName = "DC/Events/On Before Enter Tile")]
    public class OnBeforeEnterTileEventData : CrawlerEventData
    {
        [field: SerializeField] public string DungeonName { get; private set; }
        [field: SerializeField] public List<Vector2Int> Positions { get; private set; }
        [field: SerializeField] public bool CancelMoveIfTriggered { get; private set; }
    }
}