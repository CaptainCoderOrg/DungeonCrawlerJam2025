using System;
using System.Collections.Generic;
using System.Linq;

using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Unity.Assertions;

using NaughtyAttributes;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class HeroTurnController : MonoBehaviour
    {
        private static readonly Facing[] Facings = new[] { Facing.North, Facing.East, Facing.South, Facing.West };
        private static readonly Vector2Int[] Directions = new Vector2Int[]
        {
            new(-1, -1), new( 0, -1), new( 1, -1),
            new(-1,  0),              new( 1,  0),
            new(-1,  1), new( 0,  1), new( 1,  1),
        };
        private EncounterController _controller;
        private EncounterController Controller => _controller = (_controller == null ? GetComponentInParent<EncounterController>() : _controller);
        private EncounterState State => Controller.State;
        [AssertIsSet][SerializeField] private AttackConfirmationDialogue _attackConfirmationDialogue;
        [field: SerializeField] public EncounterFigureController FigureController { get; private set; }
        private MoveInfo _currentMoveInfo;
        private HashSet<MoveInfo> _possibleMoves;
        private HashSet<AttackInfo> _possibleAttacks;
        private Action<EncounterFigureController> _onSelectionChanged;
        private Action<EncounterFigureController> OnSelectionChanged
        {
            get => _onSelectionChanged;
            set
            {
                Controller.OnFigureSelected -= _onSelectionChanged;
                _onSelectionChanged = value;
                if (_onSelectionChanged != null)
                {
                    Controller.OnFigureSelected += _onSelectionChanged;
                }
            }
        }

        public event System.Action<AttackTargetSelectedEvent> OnAttackTargetSelected;

        public void CloseAttackPanel()
        {
            ClearTiles();
            OnSelectionChanged = null;
        }

        public void ShowPossibleAttacks(FigureData attacker, AttackData attack)
        {
            if (attacker == null || attack == null)
            {
                Debug.LogWarning("Attacker / Attack must be non-null", this);
                Debug.LogWarning($"Attacker: {attacker}", attacker);
                Debug.LogWarning($"Attack: {attack}", attack);
                return;
            }
            _possibleAttacks = FindAttackTargets(attacker, attack, State, Controller.EncounterData);
            HighlightAttacks(_possibleAttacks);
            OnSelectionChanged = HandleAttackTargetSelected;
            if (Controller.Selected != null)
            {
                HandleAttackTargetSelected(Controller.Selected);
            }
        }

        private void HandleAttackTargetSelected(EncounterFigureController target)
        {
            if (_possibleAttacks == null)
            {
                OnAttackTargetSelected?.Invoke(NoAttackTargetSelected.Instance);
                return;
            }
            foreach (var attack in _possibleAttacks)
            {
                if (attack.Target == target)
                {
                    OnAttackTargetSelected?.Invoke(new ValidAttackTargetSelected(attack));
                    return;
                }
            }
            OnAttackTargetSelected?.Invoke(new InvalidAttackTargetSelected(target, "Invalid Target"));
        }

        private void HighlightAttacks(HashSet<AttackInfo> attacks)
        {
            ClearTiles();
            foreach (AttackInfo attack in attacks)
            {
                if (Controller.TileSelectors.TryGetValue(attack.TargetPosition, out EncounterTileSelector tile))
                {
                    if (attack.Target == null) { tile.ShowAttackRange(); }
                    else
                    {
                        tile.ValidAttackTarget();
                    }
                }
            }
        }

        internal static HashSet<AttackInfo> FindAttackTargets(FigureData attacker, AttackData attackData, EncounterState state, EncounterData data)
        {
            int count = 0;
            HashSet<AttackInfo> possiblePositions = new();
            HashSet<Vector2Int> visited = new() { attacker.Position };
            Queue<AttackInfo> queue = new();
            queue.Enqueue(new AttackInfo(attacker.Position, attacker.Position, null, 0));
            while (queue.TryDequeue(out AttackInfo currentPosition) && count++ < 1000)
            {
                foreach (AttackInfo neighbor in GetNeighbors(currentPosition))
                {
                    if (visited.Contains(neighbor.TargetPosition)) { continue; }
                    visited.Add(neighbor.TargetPosition);
                    possiblePositions.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
            if (count >= 1000)
            {
                Debug.LogWarning($"Search exceeded expected size");
            }
            return possiblePositions;

            IEnumerable<AttackInfo> GetNeighbors(AttackInfo p)
            {
                // If the attack is not ranged, max distance is 1
                if (!attackData.AttackType.IsRanged && p.Distance >= 1) { yield break; }
                int distance = p.Distance + 1;
                foreach (Vector2Int delta in Directions)
                {

                    Vector2Int afterStep = p.TargetPosition + delta;
                    // Cannot pass through walls
                    if (data.DungeonCrawlerData.CurrentDungeon.IntersectsWall(p.StartPosition, afterStep)) { continue; }

                    // If line of site is required, check to see if there is a figure in between
                    if (attackData.AttackType.RequiresLineOfSight && state.IsFigureInBetween(p.StartPosition, afterStep)) { continue; }

                    if (state.Figures.TryGetValue(afterStep, out EncounterFigureController otherfigure))
                    {
                        yield return p with { TargetPosition = afterStep, Distance = distance, Target = otherfigure };
                    }
                    else
                    {
                        yield return p with { TargetPosition = afterStep, Distance = distance, Target = null };
                    }

                }
            }
        }

        [Button]
        public void EndPlayerTurn()
        {
            Controller.EndPlayerTurn();
        }

        internal void BeginTurn(EncounterFigureController figureController, IEnumerable<TacticData> tactics)
        {
            Controller.TacticsMenu.Cancel();
            FigureController = figureController;
            foreach (var tactic in tactics)
            {
                tactic.Effect.OnTurnStart(FigureController.Figure);
            }
            foreach (var panel in Controller.HeroPanels)
            {
                panel.StartTurn(figureController);
            }
        }

        internal void ShowMove()
        {
            _currentMoveInfo = null;
            _possibleMoves = FigureController.Figure.FindMoves(State, Controller.EncounterData);
            HashSet<Vector2Int> positions = _possibleMoves.Select(p => p.Position).ToHashSet();
            ClearTiles();
            foreach (MoveInfo moveInfo in _possibleMoves)
            {
                if (Controller.TileSelectors.TryGetValue(moveInfo.Position, out var selector))
                {
                    selector.ClearEvents();
                    selector.Highlight();
                    selector.OnMouseEntered += () => ShowMoveInfo(moveInfo);
                    selector.OnMouseExited += () => ClearMoveInfo(positions);
                    selector.OnClicked += () => PerformMove(moveInfo);
                }
            }
        }

        private void ClearTiles()
        {
            if (_currentMoveInfo != null)
            {
                foreach (Vector2Int position in _currentMoveInfo.Path())
                {
                    if (Controller.TileSelectors.TryGetValue(position, out var selector))
                    {
                        selector.ClearEvents();
                        selector.Hide();
                    }
                }
            }
            if (_possibleMoves != null)
            {
                foreach (MoveInfo info in _possibleMoves)
                {
                    if (Controller.TileSelectors.TryGetValue(info.Position, out var selector))
                    {
                        selector.ClearEvents();
                        selector.Hide();
                    }
                }
            }
            if (_possibleAttacks != null)
            {
                foreach (AttackInfo info in _possibleAttacks)
                {
                    if (Controller.TileSelectors.TryGetValue(info.TargetPosition, out var selector))
                    {
                        selector.ClearEvents();
                        selector.Hide();
                    }
                }
            }
        }

        private void PerformMove(MoveInfo moveInfo)
        {
            FigureController.Figure.Movement -= moveInfo.Distance;
            ClearTiles();
            Controller.HandleMovementEvent(new MoveFigureEvent(FigureController, moveInfo.Path().Reverse()));
        }

        private void ClearMoveInfo(HashSet<Vector2Int> possibleMoves)
        {
            if (_currentMoveInfo == null) { return; }
            foreach (Vector2Int position in _currentMoveInfo.Path())
            {
                EncounterTileSelector selector = Controller.TileSelectors[position];
                if (!possibleMoves.Contains(position)) { selector.Hide(); }
                else { selector.Highlight(); }
            }
        }
        private void ShowMoveInfo(MoveInfo moveInfo)
        {
            _currentMoveInfo = moveInfo;
            foreach (Vector2Int position in moveInfo.Path())
            {
                Controller.TileSelectors[position].Selected();
            }
        }

        

        internal void EndTurn()
        {
            OnSelectionChanged = null;
            ClearTiles();
            FigureController.Figure.Movement = 0;
            FigureController.Figure.Attacks = 0;
            FigureController.Figure.HasTakenTurn = true;
            foreach (var panel in Controller.HeroPanels)
            {
                panel.TurnEnded();
            }
            FigureController = null;
            _diceHUD.Hide();
        }

        internal void StartAttack()
        {
            _attackConfirmationDialogue.ClearTarget();
            _attackConfirmationDialogue.Attacker = FigureController.Figure;
            _attackConfirmationDialogue.Show();
        }

        [AssertIsSet][SerializeField] private DiceHUD _diceHUD;
        internal void ConfirmAttack(FigureData attacker, AttackData attack, AttackInfo attackInfo, List<DieData> attackDice)
        {
            if (attacker.Attacks < 1) { return; }
            attacker.Attacks--;
            _diceHUD.Attack = attack;
            _diceHUD.Attacker = attacker;
            _diceHUD.AttackInfo = attackInfo;
            _diceHUD.SetDice(attackDice);
            _diceHUD.Show();
            _diceHUD.Roll();
        }
    }

    public static class DungeonExtensions
    {
        const float FigureRadius = 0.500f;
        public static bool IsPassable(this Dungeon dungeon, Vector2Int position, Facing facing) => dungeon.IsPassable(new Position(position.x, position.y), facing);

        /// <summary>
        /// Calculates if two tiles have a wall obscuring sight. If any corner on the first tile has no wall to any corner on the second tile, then the tile is not obscured.
        /// </summary>
        /// <param name="dungeon"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static bool IntersectsWall(this Dungeon dungeon, Vector2Int start, Vector2Int end)
        {
            IEnumerable<LineSegment> cornerToCornerPermuations = start.CornersToCenter(end);
            foreach (LineSegment cornerToCorner in cornerToCornerPermuations)
            {
                IEnumerable<LineSegment> wallSegements = dungeon.GetWallSegments(cornerToCorner.GetGridPositions());
                // If there are no wall segements between these two corners, the tiles are visible to each other
                if (!wallSegements.Any(seg => cornerToCorner.Intersects(seg))) { return false; }
            }
            return true;
        }

        public static bool IsFigureInBetween(this EncounterState state, Vector2Int start, Vector2Int end)
        {
            LineSegment segment = new(start, end);
            return segment.GetGridPositions().Where(p => p != start && p != end).Where(state.Figures.ContainsKey).Any(c => segment.IntersectsCircle(c, FigureRadius));
        }

        public static Vector2Int Step(this Vector2Int position, Facing f) => f switch
        {
            Facing.North => new(position.x, position.y - 1),
            Facing.South => new(position.x, position.y + 1),
            Facing.East => new(position.x + 1, position.y),
            Facing.West => new(position.x - 1, position.y),
            _ => throw new Exception($"Unexpected facing: {f}"),
        };
    }

}