using System.Collections;
using System.Collections.Generic;
using System.Linq;

using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.DungeonMap.Unity;
using CaptainCoder.Dungeoneering.Unity;
using CaptainCoder.Dungeoneering.Unity.Data;
using CaptainCoder.Unity.Assertions;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class EncounterController : MonoBehaviour
    {
        [AssertIsSet][SerializeField] private ScreenHider _screenHider;
        [AssertIsSet][field: SerializeField] public SelectTacticsMenu TacticsMenu { get; private set; }
        [AssertIsSet][field: SerializeField] public HeroFigurePanel[] HeroPanels { get; private set; }
        [AssertIsSet][SerializeField] private EncounterSettingsData _encounterSettingsData;
        [AssertIsSet][SerializeField] private EncounterInitializer _initializer;
        [AssertIsSet][SerializeField] private HeroTurnController _heroTurnController;
        [AssertIsSet][field: SerializeField] public EncounterCamera EncounterCamera { get; private set; }
        public EncounterData EncounterData => _encounterSettingsData.TargetEncounter;
        private readonly DungeonBuilder _builder = new();
        [AssertIsSet][SerializeField] private Transform _tileContainer;
        [AssertIsSet][SerializeField] private DungeonTile _tilePrefab;
        [SerializeField] private EncounterFigureController _selected;
        public EncounterFigureController Selected => _selected;
        private EncounterState _state;
        public EncounterState State { get => _state ??= new(); }
        public HeroTurnController HeroTurnController => _heroTurnController;
        public Dictionary<Vector2Int, EncounterTileSelector> TileSelectors { get; private set; }
        public event System.Action<EncounterFigureController> OnFigureSelected;
        public event System.Action<EncounterController> OnEncounterLoaded;

        public void Select(EncounterFigureController selected)
        {
            _selected?.Deselect();
            _selected = selected;
            _selected?.Select();
            EncounterCamera.PanTo(_selected);
            OnFigureSelected?.Invoke(selected);
        }

        void Awake()
        {
            _builder.MinX = EncounterData.MinX;
            _builder.MaxX = EncounterData.MaxX;
            _builder.MinY = EncounterData.MinY;
            _builder.MaxY = EncounterData.MaxY;
            StartCoroutine(BuildAtEndOfFrame());
        }

        public Vector2Int FindCenter()
        {
            int minX = int.MaxValue;
            int minY = int.MaxValue;
            int maxX = int.MinValue;
            int maxY = int.MinValue;
            foreach (Vector2Int position in TileSelectors.Keys)
            {
                minX = Mathf.Min(position.x, minX);
                maxX = Mathf.Max(position.x, maxX);
                minY = Mathf.Min(position.y, minY);
                maxY = Mathf.Max(position.y, maxY);
            }
            return new Vector2Int(minX + ((maxX - minX) / 2), minY + ((maxY - minY) / 2));
        }

        private IEnumerator BuildAtEndOfFrame()
        {
            // Cannot load the dungeon until after the scene has started so we must wait 2 frames
            yield return null;
            yield return null;
            Build();
            yield return null;
            _screenHider.Show();
        }

        private void Build()
        {
            TileSelectors ??= new();
            TileSelectors.Clear();
            EncounterData.DungeonCrawlerData.LoadDungeonByName(EncounterData.DungeonName);
            _builder.BuildOrUpdateTiles(_tileContainer, _tilePrefab, UpdateTile, CreateTile);
            _initializer.Init(EncounterData);
            EncounterCamera.CenterAt(FindCenter());
        }

        private DungeonTile CreateTile(DungeonTile tilePrefab, Transform parent, Position position)
        {
            DungeonTile created = DungeonTile.Create(tilePrefab, parent, EncounterData.DungeonCrawlerData, position);
            created.transform.localPosition = new Vector3(position.Y, 0, position.X);
            EncounterTileSelector selector = created.GetComponentInChildren<EncounterTileSelector>();
            TileSelectors[new Vector2Int(position.X, position.Y)] = selector;
            return created;
        }
        private void UpdateTile(DungeonTile tile, Position position) => DungeonTile.UpdateTile(EncounterData.DungeonCrawlerData, position, tile);


        public void HandleMovementEvent(MoveFigureEvent @event)
        {
            Vector2Int start = @event.Path.First();
            Vector2Int last = @event.Path.Last();
            if (!State.Figures.TryGetValue(start, out EncounterFigureController controller))
            {
                throw new System.Exception($"Illegal movement. No figure found at position {start}.");
            }
            if (controller != @event.Controller)
            {
                throw new System.Exception($"Illegal movement. Expected {@event.Controller} ({@event.Controller.GetInstanceID()}) at position {start}.");
            }
            if (State.Figures.ContainsKey(last))
            {
                throw new System.Exception($"Illegal movement. Figure found at end position {last}.");
            }
            State.Figures.Remove(start);
            State.Figures[last] = controller;
            controller.Figure.Position = last;
            StartCoroutine(AnimateMove(controller, @event.Path));
        }

        private IEnumerator<YieldInstruction> AnimateMove(EncounterFigureController controller, IEnumerable<Vector2Int> path)
        {
            float moveTime = 0;
            foreach (Vector2Int position in path)
            {
                Vector3 start = controller.transform.localPosition;
                Vector3 end = new(position.y, 0, position.x);
                moveTime += _encounterSettingsData.MovementSpeed;
                while (moveTime > 0)
                {
                    moveTime -= Time.deltaTime;
                    float percent = 1 - (moveTime / _encounterSettingsData.MovementSpeed);
                    controller.transform.localPosition = Vector3.Lerp(start, end, percent);
                    yield return null;
                }
                controller.transform.localPosition = end;
            }
        }

        internal void SelectTactics(HeroFigurePanel heroFigurePanel)
        {
            if (_heroTurnController.FigureController != null) { return; }
            Select(heroFigurePanel.FigureController);
            TacticsMenu.SelectAndShow(heroFigurePanel);
        }
    }
}