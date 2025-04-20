
using System;

using CaptainCoder.Dungeoneering.CrawlingMode;
using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.Encounter;
using CaptainCoder.Dungeoneering.Player;
using CaptainCoder.Dungeoneering.Unity.Data;
using CaptainCoder.Unity.Assertions;

using UnityEngine;
using UnityEngine.UI;
namespace CaptainCoder.Dungeoneering.Unity
{

    public class AutoMapper : MonoBehaviour
    {
        private int _width = 11;
        private int _height = 11;
        private int _center = 5;
        [SerializeField] private AutoMapperTile[] _tiles;
        [AssertIsSet][SerializeField] private PlayerViewData _playerViewData;
        [AssertIsSet][SerializeField] private DungeonCrawlerData _dungeonCrawlerData;
        [AssertIsSet][SerializeField] private Image _playerTriangle;
        [AssertIsSet][SerializeField] private ToggleablePanel _toggleablePanel;
        [AssertIsSet][SerializeField] private OptionsSettings _optionSettings;


        void OnEnable()
        {
            _toggleablePanel.OnChange += HandleChange;
            _toggleablePanel.IsEnabled = _optionSettings.MapOpen;
            _playerViewData.OnChange.AddListener(HandleChange);
            Render(_playerViewData.View, _playerViewData);
        }

        private void HandleChange()
        {
            _optionSettings.MapOpen = _toggleablePanel.IsEnabled;
        }

        void OnDisable()
        {
            _playerViewData.OnChange.RemoveListener(HandleChange);
            _toggleablePanel.OnChange -= HandleChange;
        }

        private void HandleChange(PlayerView _, PlayerView current, PlayerViewData viewData) => Render(current, viewData);


        public void Render(PlayerView playerView, PlayerViewData playerViewData)
        {
            _playerTriangle.transform.rotation = GetRotation(playerView.Facing);
            (int r, int c) = (playerView.Position.X, playerView.Position.Y);
            for (int row = r - _center, y = 0; row < r + _center + 1; row++, y++)
            {
                for (int col = c - _center, x = 0; col < c + _center + 1; col++, x++)
                {
                    DungeonMap.Tile t = _dungeonCrawlerData.CurrentDungeon.GetTile(new DungeonMap.Position(row, col));
                    AutoMapperTile mapperTile = _tiles[x * _width + y];
                    mapperTile.Render(playerViewData.DungeonName, t);
                }
            }
        }

        private Quaternion GetRotation(Facing facing)
        {
            return facing switch
            {
                Facing.North => Quaternion.Euler(0, 0, 0),
                Facing.South => Quaternion.Euler(0, 0, 180),
                Facing.East => Quaternion.Euler(0, 0, 270),
                Facing.West => Quaternion.Euler(0, 0, 90),
                _ => throw new Exception($"Unknown rotation!"),
            };
        }
    }
}