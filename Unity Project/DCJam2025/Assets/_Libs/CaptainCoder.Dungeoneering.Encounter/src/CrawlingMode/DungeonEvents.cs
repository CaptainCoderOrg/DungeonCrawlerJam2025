using System.Collections.Generic;

using CaptainCoder.Dungeoneering.Player;
using CaptainCoder.Dungeoneering.Unity;

using NaughtyAttributes;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.CrawlingMode
{
    [CreateAssetMenu(menuName = "DC/Events/Dungeon Events")]
    public class DungeonEvents : CrawlerEventData
    {
        [field: SerializeField] public string DungeonName { get; private set; }
        [field: Expandable][field: SerializeField] public List<CrawlerEventData> Events { get; private set; }

        private readonly Dictionary<Vector2Int, CrawlerEventData> _locationBasedEvents = new();

        internal IEnumerable<EventActionData> GetEventActionsOnViewChanged(PlayerView prev, PlayerView curr, PlayerViewData playerViewData)
        {
            if (_locationBasedEvents.TryGetValue(new Vector2Int(curr.Position.X, curr.Position.Y), out CrawlerEventData eventData))
            {
                foreach (EventActionData action in eventData.Actions)
                {
                    yield return action;
                }
            }
        }

        public override void OnAfterEnterPlayMode()
        {
            base.OnAfterEnterPlayMode();
            BuildLocationBasedEvents();
        }

        private void BuildLocationBasedEvents()
        {
            _locationBasedEvents.Clear();
            foreach (CrawlerEventData @event in Events)
            {
                if (@event is CrawlerEnterTileEventData enterEvent)
                {
                    foreach (Vector2Int position in enterEvent.Positions)
                    {
                        _locationBasedEvents[position] = enterEvent;
                    }
                }
            }
        }

        void OnValidate()
        {
            BuildLocationBasedEvents();
        }
    }
}