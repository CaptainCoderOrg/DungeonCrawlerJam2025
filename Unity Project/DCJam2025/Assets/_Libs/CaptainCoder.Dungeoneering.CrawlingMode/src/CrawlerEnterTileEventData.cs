using System.Collections.Generic;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.CrawlingMode
{
    [CreateAssetMenu(menuName = "DC/Events/Enter Tile Event")]
    public class CrawlerEnterTileEventData : CrawlerEventData
    {
        [field: SerializeField] public string DungeonName { get; private set; }
        [field: SerializeField] public List<Vector2Int> Positions { get; private set; }
    }
}