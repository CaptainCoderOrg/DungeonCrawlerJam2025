using System.Collections.Generic;

using NaughtyAttributes;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.CrawlingMode
{
    [CreateAssetMenu(menuName = "DC/Events/Enter Tile Event")]
    public class CrawlerEnterTileEventData : CrawlerEventData
    {
        [field: SerializeField] public string DungeonName { get; private set; }
        [field: SerializeField] public List<Vector2Int> Positions { get; private set; }
        public Vector2Int TopLeft;
        public Vector2Int BottomRight;
        [Button]
        public void UseRectangle()
        {
            Positions.Clear();
            for (int x = TopLeft.x; x < BottomRight.x; x++)
            {
                for (int y = TopLeft.y; y < BottomRight.y; y++)
                {
                    Positions.Add(new Vector2Int(x, y));
                }
            }
        }
    }
}