using System;

using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.Player;
using CaptainCoder.Dungeoneering.Unity.Data;
using CaptainCoder.Unity;

using UnityEngine;
using UnityEngine.InputSystem;

namespace CaptainCoder.Dungeoneering.Unity
{

    public class PlayerViewController : MonoBehaviour
    {
        [field: SerializeField]
        public PlayerViewData PlayerView { get; private set; }
        [SerializeField] private bool _isInEditor = true;
        [SerializeField]
        private DungeonCrawlerData _dungeonCrawlerData;
        public Func<PlayerView, PlayerView, bool> ValidateMove;
        void Awake()
        {
            Assertion.NotNull(this, (_dungeonCrawlerData, "Dungeon Crawler Data"));
        }
        public void HandleInput(MovementAction action)
        {
            if (_isInEditor && Keyboard.current.shiftKey.isPressed)
            {
                HandleInputIgnoringWalls(action);
                return;
            }
            PlayerView result = PlayerControls.Move(_dungeonCrawlerData.CurrentDungeon, PlayerView.View, action);
            if (ValidateMove == null || ValidateMove.Invoke(PlayerView.View, result))
            {
                PlayerView.View = result;
            }
        }

        private void HandleInputIgnoringWalls(MovementAction action)
        {
            (Position p, Facing f) = PlayerView.View;
            PlayerView newView = action switch
            {
                MovementAction.StepForward => new PlayerView(p.Step(f), f),
                MovementAction.StepBackward => new PlayerView(p.Step(f.Opposite()), f),
                MovementAction.StrafeLeft => new PlayerView(p.Step(f.RotateCounterClockwise()), f),
                MovementAction.StrafeRight => new PlayerView(p.Step(f.Rotate()), f),
                _ => PlayerControls.Move(_dungeonCrawlerData.CurrentDungeon, PlayerView.View, action),
            };
            PlayerView.View = newView;
        }
    }

}