using System.Collections;
using System.Collections.Generic;

using CaptainCoder.Dungeoneering.DungeonMap.Unity;
using CaptainCoder.Dungeoneering.Encounter;
using CaptainCoder.Dungeoneering.Player;
using CaptainCoder.Dungeoneering.Unity;
using CaptainCoder.Unity.Assertions;

using NaughtyAttributes;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace CaptainCoder.Dungeoneering.CrawlingMode
{

    public class CrawlerLogicController : MonoBehaviour
    {
        [AssertIsSet][SerializeField] private ScreenHider _hider;
        [AssertIsSet][SerializeField] private EncounterSettingsData _encounterSettingsData;
        [AssertIsSet][SerializeField] private DungeonController _dungeonController;
        [AssertIsSet][SerializeField] private PlayerViewData _playerViewData;
        [Expandable][AssertIsSet][SerializeField] private List<CrawlerEventData> _events;

        void Awake()
        {
            _playerViewData.OnChange.AddListener(HandlePlayerViewChanged);
        }

        private void HandlePlayerViewChanged(PlayerView prev, PlayerView curr)
        {
            if (prev.Position == curr.Position) { return; }
            // TODO: Optimize looking up events
            foreach (CrawlerEventData @event in _events)
            {
                if (@event is CrawlerEnterTileEventData enterEvent && enterEvent.DungeonName == _dungeonController.DungeonCrawlerData.CurrentDungeon.Name)
                {
                    foreach (var position in enterEvent.Positions)
                    {
                        if (position.x == curr.Position.X && position.y == curr.Position.Y)
                        {
                            foreach (var action in @event.Actions)
                            {
                                action.Execute(this);
                            }
                        }
                    }
                }
            }
        }

        public void StartEncounter(EncounterData encounterData)
        {
            _encounterSettingsData.TargetEncounter = encounterData;
            _hider.OnFinished += StartLoadEncounter;
            _hider.Hide();
        }

        private void StartLoadEncounter()
        {
            StartCoroutine(LoadEncounter());
            _hider.OnFinished -= StartLoadEncounter;
        }

        private IEnumerator LoadEncounter()
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync("Encounter");
            while (!operation.isDone)
            {
                yield return null;
            }
        }
    }
}