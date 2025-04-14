using System.Collections;
using System.Collections.Generic;

using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.DungeonMap.Unity;
using CaptainCoder.Dungeoneering.Encounter;
using CaptainCoder.Dungeoneering.Player;
using CaptainCoder.Dungeoneering.Unity;
using CaptainCoder.Dungeoneering.Unity.Data;
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

        [Expandable][SerializeField] private EventActionData _testAction;
        void Awake()
        {
            _playerViewData.OnChange.AddListener(HandlePlayerViewChanged);
            _playerViewData.OnDungeonChanged.AddListener(HandleDungeonChanged);
        }

        private void HandleDungeonChanged(string dungeonName)
        {
            _dungeonController.DungeonCrawlerData.LoadDungeonByName(dungeonName);
        }

        private void HandlePlayerViewChanged(PlayerView prev, PlayerView curr, PlayerViewData playerViewData)
        {
            if (prev.Position == curr.Position) { return; }
            // TODO: Optimize looking up events
            foreach (CrawlerEventData @event in _events)
            {
                if (@event is DungeonEvents dungeonEvents)
                {
                    if (dungeonEvents.DungeonName == playerViewData.DungeonName)
                    {
                        foreach (var action in dungeonEvents.GetEventActionsOnViewChanged(prev, curr, playerViewData))
                        {
                            action.Execute(this);
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

        [Button]
        public void ExecuteTestAction()
        {
            _testAction.Execute(this);
        }

        public void Teleport(string dungeonName, int x, int y, Facing facing)
        {
            _playerViewData.DungeonName = dungeonName;
            _playerViewData.View = new PlayerView(x, y, facing);
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