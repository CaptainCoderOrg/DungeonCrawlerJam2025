using System.Collections;
using System.Collections.Generic;
using System.Linq;

using CaptainCoder.Dungeoneering.CrawlingMode;
using CaptainCoder.Unity.Assertions;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class EnemyTurnController : MonoBehaviour
    {
        [AssertIsSet][SerializeField] private GuardWindowController _guardWindowController;
        [AssertIsSet][SerializeField] private DiceHUD _diceHUD;
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
                    yield return StartCoroutine(TryAttackHero(figure, enemyData));
                    if (Controller.CheckForDefeat())
                    {
                        Controller.TileHighlighter.ClearAll();
                        Controller.EndEnemyTurn();
                        yield break;
                    }
                    yield return StartCoroutine(TryMoveAwayFromHero(figure, enemyData));
                }
                Controller.TileHighlighter.ClearAll();
            }
            Controller.EndEnemyTurn();
        }


        private IEnumerator TryToMoveToHero(EncounterFigureController enemyFigure, EnemyEntityData enemy)
        {
            if (enemy.Health <= 0)
            {
                Controller.TileHighlighter.ClearAll();
                yield break;
            }
            if (IsAdjacentHero(enemyFigure.Figure.Position, State)) { yield break; }
            HashSet<MoveInfo> possibleMoves = enemyFigure.Figure.FindMoves(State, Controller.EncounterData);
            if (TryFindClosestMove(possibleMoves, State, out MoveInfo target))
            {
                Controller.TileHighlighter.Highlight(possibleMoves.Select(p => p.Position));
                Controller.TileHighlighter.Selected(target.Path());
                yield return StartCoroutine(CheckGuard(enemyFigure, target.Path()));
                if (enemy.Health <= 0)
                {
                    Controller.TileHighlighter.ClearAll();
                    yield break;
                }
                yield return Settings.EnemyDelay;
                enemyFigure.Figure.Movement -= target.Distance;
                yield return StartCoroutine(Controller.HandleMovementEvent(new MoveFigureEvent(enemyFigure, target.Path().Reverse())));
                Controller.TileHighlighter.ClearAll();
            }
        }

        private IEnumerator CheckGuard(EncounterFigureController attacker, IEnumerable<Vector2Int> positions)
        {
            IEnumerable<GuardInfo> guardingHeroes = Controller.HeroPanels
                                                        .Where(h => h.gameObject.activeInHierarchy && h.FigureController.Figure.EntityData.Effects.Any(e => e.IsGuard))
                                                        .Select(p => p.FindGuardPositions(attacker, State, Controller.EncounterData, positions))
                                                        .Where(g => g.HasTargets());
            if (guardingHeroes.Any())
            {
                Debug.Log($"Guarding: {string.Join(", ", guardingHeroes.Select(g => g.Hero.FigureController.Figure.EntityData.Name))}");
                yield return StartCoroutine(_guardWindowController.ShowGuardWindowAndWait(guardingHeroes));

            }
            yield return null;
        }


        private IEnumerator TryMoveAwayFromHero(EncounterFigureController enemyFigure, EnemyEntityData enemy)
        {
            if (enemy.Health <= 0)
            {
                Controller.TileHighlighter.ClearAll();
                yield break;
            }
            HashSet<MoveInfo> possibleMoves = enemyFigure.Figure.FindMoves(State, Controller.EncounterData);
            if (TryFindFurthestMove(possibleMoves, State, out MoveInfo furthest))
            {
                Controller.TileHighlighter.Highlight(possibleMoves.Select(p => p.Position));
                Controller.TileHighlighter.Selected(furthest.Path());
                yield return StartCoroutine(CheckGuard(enemyFigure, furthest.Path()));
                if (enemy.Health <= 0)
                {
                    Controller.TileHighlighter.ClearAll();
                    yield break;
                }
                yield return Settings.EnemyDelay;
                Controller.TileHighlighter.ClearAll();
                enemyFigure.Figure.Movement -= furthest.Distance;
                yield return StartCoroutine(Controller.HandleMovementEvent(new MoveFigureEvent(enemyFigure, furthest.Path().Reverse())));
            }
        }

        public static bool TryFindClosestMove(HashSet<MoveInfo> move, EncounterState state, out MoveInfo closest)
        {
            IEnumerable<(MoveInfo, float)> info = move.Select(m => (m, DistanceToNearestHero(m, state))).OrderBy(m => m.Item2);
            closest = info.FirstOrDefault().Item1;
            return closest != null;
        }

        public static bool TryFindFurthestMove(HashSet<MoveInfo> move, EncounterState state, out MoveInfo furthest)
        {
            IEnumerable<(MoveInfo, float)> info = move.Select(m => (m, DistanceToNearestHero(m, state))).OrderByDescending(m => m.Item2);
            furthest = info.FirstOrDefault().Item1;
            return furthest != null;
        }

        public static float DistanceToNearestHero(MoveInfo move, EncounterState state)
        {
            IEnumerable<float> distances = state.Figures.Where(k => k.Value.Figure.EntityData is HeroEntityData).Select(k => (k.Key - move.Position).magnitude);
            return distances.Any() ? distances.Min() : 0;
        }

        private IEnumerator TryAttackHero(EncounterFigureController enemyFigure, EnemyEntityData enemy)
        {
            if (enemy.Health <= 0)
            {
                Controller.TileHighlighter.ClearAll();
                yield break;
            }
            AttackData attack = enemy.Attacks[Random.Range(0, enemy.Attacks.Count)];
            HashSet<AttackInfo> possibleAttacks = enemyFigure.Figure.FindAttackTargets(attack, State, Controller.EncounterData);
            if (TrySelectTarget(possibleAttacks, out AttackInfo target))
            {
                Controller.TileHighlighter.HighlightAttacks(possibleAttacks);
                Controller.TileHighlighter.Selected(Enumerable.Repeat(target.TargetPosition, 1));
                yield return StartCoroutine(CheckGuard(enemyFigure, Enumerable.Repeat(enemyFigure.Figure.Position, 1)));
                if (enemy.Health <= 0)
                {
                    Controller.TileHighlighter.ClearAll();
                    yield break;
                }
                yield return Settings.EnemyDelay;
                yield return StartCoroutine(TryEnemyAttackRoll(enemyFigure, attack, target));
            }
            else
            {
                yield return Settings.EnemyDelay;
            }
            Controller.TileHighlighter.ClearAll();
        }

        private IEnumerator TryEnemyAttackRoll(EncounterFigureController figure, AttackData attackData, AttackInfo attackInfo)
        {
            figure.Figure.Attacks--;
            _diceHUD.Attack = attackData;
            _diceHUD.Attacker = figure.Figure;
            _diceHUD.AttackInfo = attackInfo;
            _diceHUD.SetDice(Enumerable.Repeat(attackData.AttackType.AttackDie, 1).Concat(attackData.PowerDice));
            _diceHUD.Show();
            yield return StartCoroutine(_diceHUD.Roll());
            // yield return _diceHUD.AutoApplyBonuses(Settings);
            yield return _diceHUD.WaitForEnemyRollConfirmed();
            Debug.Log("Confirmed");
        }

        private bool TrySelectTarget(HashSet<AttackInfo> possibleAttacks, out AttackInfo selected)
        {
            IEnumerable<AttackInfo> targets = possibleAttacks.Where(a => a.Target != null && a.Target.Figure.EntityData is HeroEntityData).OrderBy(a => a.Distance);
            selected = targets.FirstOrDefault();
            return selected != null;
        }

        private static int DistanceToHeroes(MoveInfo move, EncounterState state)
        {
            // TODO: Determine distance to each hero
            return 5;
        }

        private static bool IsAdjacentHero(Vector2Int move, EncounterState state)
        {
            foreach (Vector2Int neighbor in GetNeighbors(move))
            {
                if (state.Figures.TryGetValue(neighbor, out EncounterFigureController controller) && controller.Figure.EntityData is HeroEntityData)
                {
                    return true;
                }
            }
            return false;
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