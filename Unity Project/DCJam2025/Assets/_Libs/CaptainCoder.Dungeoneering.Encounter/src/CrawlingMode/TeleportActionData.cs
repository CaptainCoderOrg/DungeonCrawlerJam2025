using CaptainCoder.Dungeoneering.DungeonMap;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.CrawlingMode
{
    [CreateAssetMenu(menuName = "DC/Events/Action/Teleport Action")]
    public class TeleportActionData : EventActionData
    {
        [SerializeField] private string _dungeonName;
        [SerializeField] private int _x;
        [SerializeField] private int _y;
        [SerializeField] private Facing _facing;
        [field: SerializeField] public Texture2D CeilingTile { get; private set; }

        public override void Execute(CrawlerLogicController controller)
        {
            base.Execute(controller);
            controller.Teleport(_dungeonName, _x, _y, _facing, CeilingTile);
        }
    }
}