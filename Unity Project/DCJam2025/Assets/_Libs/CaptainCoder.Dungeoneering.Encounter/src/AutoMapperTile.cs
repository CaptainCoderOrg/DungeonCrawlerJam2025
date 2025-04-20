
using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Unity.Assertions;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Unity
{

    public class AutoMapperTile : MonoBehaviour
    {
        [AssertIsSet][SerializeField] private PlayerViewData _playerViewData;
        [AssertIsSet][SerializeField] private CanvasGroup _northWall;
        [AssertIsSet][SerializeField] private CanvasGroup _eastWall;
        [AssertIsSet][SerializeField] private CanvasGroup _southWall;
        [AssertIsSet][SerializeField] private CanvasGroup _westWall;
        [AssertIsSet][SerializeField] private CanvasGroup _northDoor;
        [AssertIsSet][SerializeField] private CanvasGroup _eastDoor;
        [AssertIsSet][SerializeField] private CanvasGroup _southDoor;
        [AssertIsSet][SerializeField] private CanvasGroup _westDoor;
        [AssertIsSet][SerializeField] private CanvasGroup _baseTile;
        [AssertIsSet][SerializeField] private CanvasGroup _visitedTile;

        private CanvasGroup[] _allImages;

        void Awake()
        {
            _allImages = new CanvasGroup[]{ _northWall, _eastWall, _southWall, _westWall, _northDoor, _eastDoor, _southDoor, _westDoor };
        }

        public void Render(string dungeonName, Tile tile)
        {
            if (_playerViewData.VisitedLocations.Contains(new Visited(dungeonName, tile.Position)))
            {
                _visitedTile.alpha = 1;
            }
            else
            {
                _visitedTile.alpha = 0;
            }

            _baseTile.alpha = 1;
            if (tile.Walls.North == WallType.Door)
            {
                _northDoor.alpha = 1;
                _northWall.alpha = 0;
            }
            else if (tile.Walls.North == WallType.None)
            {
                _northWall.alpha = 0;
                _northDoor.alpha = 0;
            }
            else
            {
                _northWall.alpha = 1;
                _northDoor.alpha = 0;
            }

            if (tile.Walls.East == WallType.Door)
            {
                _eastDoor.alpha = 1;
                _eastWall.alpha = 0;
            }
            else if (tile.Walls.East == WallType.None)
            {
                _eastWall.alpha = 0;
                _eastDoor.alpha = 0;
            }
            else
            {
                _eastWall.alpha = 1;
                _eastDoor.alpha = 0;
            }

            if (tile.Walls.South == WallType.Door)
            {
                _southDoor.alpha = 1;
                _southWall.alpha = 0;
            }
            else if (tile.Walls.South == WallType.None)
            {
                _southWall.alpha = 0;
                _southDoor.alpha = 0;
            }
            else
            {
                _southWall.alpha = 1;
                _southDoor.alpha = 0;
            }

            if (tile.Walls.West == WallType.Door)
            {
                _westDoor.alpha = 1;
                _westWall.alpha = 0;
            }
            else if (tile.Walls.West == WallType.None)
            {
                _westWall.alpha = 0;
                _westDoor.alpha = 0;
            }
            else
            {
                _westWall.alpha = 1;
                _westDoor.alpha = 0;
            }
        }

        private void HideAll()
        {
            _visitedTile.alpha = 1;
            _baseTile.alpha = 0;
            foreach (var image in _allImages)
            {
                image.alpha = 0;
            }
        }
    }
}