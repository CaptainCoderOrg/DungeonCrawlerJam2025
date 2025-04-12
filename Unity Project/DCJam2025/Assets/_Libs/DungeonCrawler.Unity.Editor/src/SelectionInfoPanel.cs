
using System.Collections.Generic;
using System.Linq;

using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.DungeonMap.Unity;
using CaptainCoder.Dungeoneering.Unity.Data;
using CaptainCoder.Unity.Assertions;

using TMPro;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Unity.Editor
{
    public class SelectionInfoPanel : MonoBehaviour
    {
        private static readonly Facing[] Facings = { Facing.North, Facing.East, Facing.South, Facing.West };
        [AssertIsSet][SerializeField] private DungeonCrawlerData _dungeonCrawlerData;
        [AssertIsSet][SerializeField] private DungeonEditorSelectionData _selection;
        [AssertIsSet][SerializeField] private TileTextureSelectorPanel _tileTextureSelector;
        [AssertIsSet][SerializeField] private GameObject _content;
        [AssertIsSet][SerializeField] private TextureLabelController _tilesLabel;
        [AssertIsSet][SerializeField] private TextureLabelController _wallsLabel;
        [AssertIsSet][SerializeField] private TextureLabelController _doorsLabel;
        [AssertIsSet][SerializeField] private TextureLabelController _secretDoorLabel;
        [AssertIsSet][SerializeField] private TextMeshProUGUI _selectedPosition;
        private WallSelectionData _wallSelectionData = new();

        private void HandleTilesChanged(DungeonChangeEvent changes)
        {
            if (changes is TilesChanged(IEnumerable<TileReference> _))
            {
                RenderInfo(_selection.Tiles, _selection.Walls);
            }
        }

        void OnEnable()
        {
            _dungeonCrawlerData.AddObserver(HandleTilesChanged);
            _selection.AddObserver(HandleSelectionChanged);
        }

        void OnDisable()
        {
            _dungeonCrawlerData.RemoveObserver(HandleTilesChanged);
            _selection.RemoveObserver(HandleSelectionChanged);
        }

        private void HandleSelectionChanged(SelectionChangedEvent @event)
        {
            if (@event is SelectionChanged selection)
            {
                RenderInfo(selection.Tiles, selection.Walls);
            }
        }

        private void RenderTileInfo(ISet<DungeonTile> tiles)
        {
            if (tiles.Count == 0)
            {
                _selectedPosition.text = $"Selected Position: None";
            }
            else
            {
                int minX = int.MaxValue;
                int minY = int.MaxValue;
                int maxX = int.MinValue;
                int maxY = int.MinValue;
                foreach (var tile in tiles)
                {
                    minX = Mathf.Min(tile.Position.X, minX);
                    maxX = Mathf.Max(tile.Position.X, maxX);
                    minY = Mathf.Min(tile.Position.Y, minY);
                    maxY = Mathf.Max(tile.Position.Y, maxY);
                    _selectedPosition.text = $"Selected Position: ({minX}, {minY}) - ({maxX}, {maxY})";
                }
            }
            (string tileTextureName, TextureReference tileTexture) = TextureLabel(tiles);
            _tilesLabel.Label.text = $"{tiles.Count()} Tiles: {tileTextureName}";
            _tilesLabel.Button.Texture = tileTexture;
        }

        private void RenderInfo(ISet<DungeonTile> tiles, ISet<DungeonWallController> walls)
        {
            _wallSelectionData.CountWalls(tiles, walls);
            RenderTileInfo(tiles);
            UpdateLabel(_wallsLabel, "Walls", _wallSelectionData.Solid);
            UpdateLabel(_doorsLabel, "Doors", _wallSelectionData.Doors);
            UpdateLabel(_secretDoorLabel, "Secret Doors", _wallSelectionData.SecretDoors);
            _content.SetActive(true);
        }

        private void UpdateLabel(TextureLabelController label, string name, ISet<(Position, Facing)> walls)
        {
            (string wallTextureName, TextureReference wallTexture) = TextureLabel(walls);
            label.Label.text = $"{walls.Count} {name}: {wallTextureName}";
            label.Button.Texture = wallTexture;
        }

        private (string, TextureReference) TextureLabel(ISet<(Position p, Facing f)> walls)
        {
            if (walls.Count() < 1) { return ("No Selection", null); }
            string textureName = GetTextureName(walls.First());
            if (walls.All(w => GetTextureName(w) == textureName))
            {
                return (textureName, _dungeonCrawlerData.GetTexture(textureName));
            }
            return ("Multiple textures", null);
            string GetTextureName((Position p, Facing f) wall) => _dungeonCrawlerData.GetTexture(new WallReference(_dungeonCrawlerData.CurrentDungeon, wall.p, wall.f)).TextureName;
        }

        private (string, TextureReference) TextureLabel(ISet<DungeonTile> tiles)
        {
            if (tiles.Count() < 1) { return ("No Selection", null); }
            TextureReference textureRef = _dungeonCrawlerData.GetTexture(tiles.First().TileReference);
            if (tiles.All(t => _dungeonCrawlerData.GetTexture(t.TileReference) == textureRef))
            {
                return (textureRef.TextureName, textureRef);
            }
            return ("Multiple textures", null);
        }

        private void SetTileTexture(SetTileOptions options)
        {
            if (!_selection.Tiles.Any()) { return; }
            void Perform()
            {
                Debug.Log(options);
                foreach (DungeonTile tile in _selection.Tiles)
                {
                    if (Random.Range(0, 1f) > options.Chance)
                    {
                        Debug.Log("Skipping tile");
                        continue;
                    }
                    TileReference tileRef = tile.TileReference;
                    TextureReference originalTexture = _dungeonCrawlerData.GetTexture(tileRef);
                    _dungeonCrawlerData.SetTexture(tileRef, options.Texture);
                }
            }

            _dungeonCrawlerData.PerformEditSerializeState($"Set Multiple Textures: {options.Texture.TextureName}", Perform);
        }

        private void SetSolidTextures(SetTileOptions options) => SetWallTextures(options, _wallSelectionData.Solid);
        private void SetDoorTextures(SetTileOptions options) => SetWallTextures(options, _wallSelectionData.Doors);
        private void SetSecretTextures(SetTileOptions options) => SetWallTextures(options, _wallSelectionData.SecretDoors);

        private void SetWallTextures(SetTileOptions option, ISet<(Position p, Facing f)> walls)
        {
            (Position p, Facing f)[] cachedWalls = walls.ToArray();
            void Perform()
            {
                foreach ((Position p, Facing f) in cachedWalls)
                {
                    if (Random.Range(0, 1f) > option.Chance) { continue; }
                    WallReference wallRef = new(_dungeonCrawlerData.CurrentDungeon, p, f);
                    _dungeonCrawlerData.SetTexture(wallRef, option.Texture);
                }
            }
            _dungeonCrawlerData.PerformEditSerializeState("Set Multiple Wall Textures", Perform);
        }

        public void OpenTileTextureSelector() => _tileTextureSelector.ShowTileSelection(SetTileTexture);
        public void OpenWallTextureSelector() => _tileTextureSelector.ShowWallSelection(SetSolidTextures, WallType.Solid, _wallSelectionData.Solid);
        public void OpenDoorsTextureSelector() => _tileTextureSelector.ShowWallSelection(SetDoorTextures, WallType.Door, _wallSelectionData.Doors);
        public void OpenSecretDoorsTextureSelector() => _tileTextureSelector.ShowWallSelection(SetSecretTextures, WallType.SecretDoor, _wallSelectionData.SecretDoors);
    }

    public record struct SetTileOptions(TextureReference Texture, float Chance = 1);
}