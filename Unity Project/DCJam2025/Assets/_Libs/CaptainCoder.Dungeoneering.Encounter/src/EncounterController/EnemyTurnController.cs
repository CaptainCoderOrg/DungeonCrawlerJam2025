

using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class EnemyTurnController : MonoBehaviour
    {
        private EncounterSettingsData Settings => Controller.EncounterSettingsData;
        private EncounterController _controller;
        private EncounterController Controller => _controller = (_controller == null ? GetComponentInParent<EncounterController>() : _controller);
        private EncounterState State => Controller.State;
        internal void TakeEnemyTurn()
        {
            StartCoroutine(EnemyTurnRoutine());
        }

        private IEnumerator EnemyTurnRoutine()
        {
            foreach ((Vector2Int position, EncounterFigureController figure) in State.Figures.ToArray())
            {
                if (figure.Figure.EntityData is EnemyEntityData enemyData)
                {
                    Controller.Select(figure);
                    figure.Figure.Attacks = 1;
                    figure.Figure.Movement = figure.Figure.EntityData.Speed;
                    yield return Settings.EnemyDelay;
                    yield return StartCoroutine(TryToMoveToHero(figure, enemyData));
                    Controller.TileHighlighter.ClearAll();
                }
            }
        }

        private IEnumerator TryToMoveToHero(EncounterFigureController figure, EnemyEntityData enemy)
        {
            HashSet<MoveInfo> possibleMoves = figure.Figure.FindMoves(State, Controller.EncounterData);
            Debug.Log(possibleMoves.Count);
            Controller.TileHighlighter.Highlight(possibleMoves.Select(p => p.Position));
            yield return Settings.EnemyDelay;
            IEnumerable<MoveInfo> adjacentHero = possibleMoves.Where(m => IsAdjacentHero(m, State));
            if (!adjacentHero.Any())
            {
                Debug.LogWarning("TODO: Implement AI for moving toward hero");
                yield break;
            }
            Debug.LogWarning("TODO: Implement AI for selecting hero target");
            MoveInfo move = adjacentHero.First();
            Controller.TileHighlighter.Selected(move.Path());
            yield return Settings.EnemyDelay;
            Controller.TileHighlighter.ClearAll();
            yield return Controller.HandleMovementEvent(new MoveFigureEvent(figure, move.Path().Reverse()));
        }

        private static int DistanceToHeros(MoveInfo move, EncounterState state)
        {
            // TODO: Determine distance to each hero
            return 5;
        }

        private static bool IsAdjacentHero(MoveInfo move, EncounterState state)
        {
            foreach (Vector2Int neighbor in GetNeighbors(move.Position))
            {
                if (state.Figures.TryGetValue(neighbor, out EncounterFigureController controller) && controller.Figure.EntityData is HeroEntityData)
                {
                    return true;
                }
            }
            return false;
        }

        private static readonly Vector2Int[] Directions = new Vector2Int[]
        {
            new(-1, -1), new( 0, -1), new( 1, -1),
            new(-1,  0),              new( 1,  0),
            new(-1,  1), new( 0,  1), new( 1,  1),
        };

        private static IEnumerable<Vector2Int> GetNeighbors(Vector2Int center)
        {
            foreach (Vector2Int delta in Directions)
            {
                yield return center + delta;
            }
        }

        private AttackData SelectAttack(FigureData figure, EnemyEntityData enemy, EncounterState state)
        {
            Debug.LogWarning($"TODO: Implement enemy select attack");
            return enemy.Attacks[0];
        }
    }
}