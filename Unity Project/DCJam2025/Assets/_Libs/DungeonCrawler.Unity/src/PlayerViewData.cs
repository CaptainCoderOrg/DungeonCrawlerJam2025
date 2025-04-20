using System.Collections.Generic;

using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.Player;

using UnityEngine;
using UnityEngine.Events;
namespace CaptainCoder.Dungeoneering.Unity
{
    [CreateAssetMenu(menuName = "DC/PlayerView")]
    public class PlayerViewData : ObservableSO
    {
        public HashSet<Visited> VisitedLocations { get; private set; } = new();
        public PlayerView PreviousSpace { get; private set; }
        public UnityEvent<PlayerView, PlayerView, PlayerViewData> OnChange { get; private set; } = new();
        public UnityEvent<string> OnDungeonChanged { get; private set; } = new();

        [field: SerializeField]
        public int X { get; private set; }
        [field: SerializeField]
        public int Y { get; private set; }
        [field: SerializeField]
        public Facing Facing { get; private set; }
        [SerializeField] private string _dungeonName = "Farm";
        public string DungeonName
        {
            get => _dungeonName;
            set
            {
                if (_dungeonName == value) { return; }
                _dungeonName = value;
                OnDungeonChanged?.Invoke(_dungeonName);
            }
        }

        private PlayerView _view;
        public PlayerView View
        {
            get => _view;
            set
            {
                if (_view == value) { return; }
                PreviousSpace = _view;
                _view = value;
                VisitedLocations.Add(new Visited(_dungeonName, value.Position));
                X = _view.Position.X;
                Y = _view.Position.Y;
                Facing = _view.Facing;
                OnChange.Invoke(PreviousSpace, _view, this);
            }
        }

        protected override void OnExitPlayMode()
        {
            base.OnExitPlayMode();
            OnChange.RemoveAllListeners();
            OnDungeonChanged.RemoveAllListeners();
            VisitedLocations.Clear();
        }

        public override void OnAfterEnterPlayMode()
        {
            base.OnAfterEnterPlayMode();
            View = new PlayerView(new(X, Y), Facing);
            OnDungeonChanged?.Invoke(_dungeonName);
        }

        public void OnValidate()
        {
            View = new PlayerView(new(X, Y), Facing);
            OnDungeonChanged?.Invoke(_dungeonName);
        }

        public void LoadPreviousSpace()
        {
            if (PreviousSpace == null) { return; }
            _view = PreviousSpace;
            X = _view.Position.X;
            Y = _view.Position.Y;
            Facing = _view.Facing;
            OnChange.Invoke(PreviousSpace, _view, this);
        }

        public void MoveBack()
        {
            View = PreviousSpace;
        }
    }

    public record struct Visited(string DungeonName, Position Position);
}