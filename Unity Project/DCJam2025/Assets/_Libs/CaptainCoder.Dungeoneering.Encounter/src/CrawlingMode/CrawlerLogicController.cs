using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.Encounter;
using CaptainCoder.Dungeoneering.Player;
using CaptainCoder.Dungeoneering.Unity;
using CaptainCoder.Dungeoneering.Unity.Data;
using CaptainCoder.Unity.Assertions;

using NaughtyAttributes;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace CaptainCoder.Dungeoneering.CrawlingMode
{

    public class CrawlerLogicController : MonoBehaviour
    {
        [SerializeField] private PlayerViewController _viewController;
        [SerializeField] private EncounterController _encounterController;
        [AssertIsSet][SerializeField] private ScreenHider _hider;
        [AssertIsSet][SerializeField] private EncounterSettingsData _encounterSettingsData;
        [AssertIsSet][SerializeField] private DungeonCrawlerData _dungeonCrawlerData;
        [AssertIsSet][SerializeField] private PlayerViewData _playerViewData;
        [AssertIsSet][SerializeField] private DialogueController _dialogueController;
        [Expandable][AssertIsSet][SerializeField] private List<CrawlerEventData> _events;
        [field: SerializeField] public UnityEvent<PlayerView, PlayerView, PlayerViewData> OnMove;

        [Expandable][SerializeField] private EventActionData _testAction;
        void Awake()
        {
            _playerViewData.OnChange.AddListener(HandlePlayerViewChanged);
            _playerViewData.OnDungeonChanged.AddListener(HandleDungeonChanged);
            _viewController = FindFirstObjectByType<PlayerViewController>();
            _encounterController = FindFirstObjectByType<EncounterController>();
            if (_viewController != null) { _viewController.ValidateMove = HandleBeforeMove; }
            StartCoroutine(ShowScreenAtEndOfFrame());
        }

        private bool HandleBeforeMove(PlayerView exiting, PlayerView entering)
        {
            if (_dialogueController.IsShowing) { return false; }
            // TODO: Needs optimization, we shouldn't need to iterate through all possible events
            bool canceled = false;
            foreach (OnBeforeEnterTileEventData before in BeforeEnterEvents.Where(e => e.AllConditionsMet()))
            {
                foreach (Vector2Int position in before.Positions)
                {
                    if (position.x == entering.Position.X && position.y == entering.Position.Y)
                    {
                        canceled = canceled || before.CancelMoveIfTriggered;
                        foreach (var action in before.Actions)
                        {
                            action.Execute(this);
                        }

                    }
                }
            }
            return !canceled;
        }

        private IEnumerator ShowScreenAtEndOfFrame()
        {
            yield return null;
            _hider.Show();
        }

        private void HandleDungeonChanged(string dungeonName)
        {
            _dungeonCrawlerData.LoadDungeonByName(dungeonName);
        }

        private IEnumerable<CrawlerEventData> DungeonEvents => _events.Where(e => e is DungeonEvents de && de.DungeonName == _playerViewData.DungeonName).SelectMany(e => ((DungeonEvents)e).Events);
        private IEnumerable<OnBeforeEnterTileEventData> BeforeEnterEvents => DungeonEvents.Where(e => e is OnBeforeEnterTileEventData).Select(e => e as OnBeforeEnterTileEventData);

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

        internal void ShowDialogue(Sprite portrait, string message, DialogueOption[] options)
        {
            _dialogueController.Portrait = portrait;
            _dialogueController.SetOptions(options);
            _dialogueController.ShowMessage(message);
        }

        internal void ShowDialogue(ChainedDialogueActionData chainedDialogueActionData)
        {
            _dialogueController.ShowChainedDialogue(chainedDialogueActionData);
        }

        internal void HideDialogue()
        {
            _dialogueController.Hide();
        }

        internal void EndEncounter()
        {
            StartCoroutine(EndEncounterSequence());
        }
        private IEnumerator EndEncounterSequence()
        {
            yield return StartCoroutine(_hider.ShowCoroutine());
            AsyncOperation callback = SceneManager.LoadSceneAsync("DungeonCrawling");
            while (!callback.isDone) { yield return null; }

            // TODO: This is a hack that ensure there are no left over listeners at the end of combat
            foreach (var hero in _encounterController.State.Heroes)
            {
                hero.ClearListeners();
            }
        }
    }
}