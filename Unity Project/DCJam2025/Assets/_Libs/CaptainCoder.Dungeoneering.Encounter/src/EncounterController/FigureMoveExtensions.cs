using System.Collections.Generic;

using CaptainCoder.Dungeoneering.DungeonMap;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter;

public static class FigureMoveExtensions
{
    private static readonly Facing[] Facings = new[] { Facing.North, Facing.East, Facing.South, Facing.West };
    internal static HashSet<MoveInfo> FindMoves(this FigureData figure, EncounterState state, EncounterData data)
    {
        HashSet<MoveInfo> validMoves = new();
        HashSet<Vector2Int> visited = new() { figure.Position };
        Queue<MoveInfo> queue = new();
        queue.Enqueue(new MoveInfo(figure.Position, null, 0));
        while (queue.TryDequeue(out MoveInfo currentPosition))
        {
            foreach (MoveInfo neighbor in GetNeighbors(currentPosition))
            {
                if (visited.Contains(neighbor.Position)) { continue; }
                visited.Add(neighbor.Position);
                if (!state.Figures.ContainsKey(neighbor.Position))
                {
                    // If there is no figure in this space, we can end our movement here
                    validMoves.Add(neighbor);
                }
                queue.Enqueue(neighbor);
            }
        }
        return validMoves;

        IEnumerable<MoveInfo> GetNeighbors(MoveInfo p)
        {
            if (p.Distance >= figure.Movement) { yield break; }
            int distance = p.Distance + 1;
            foreach (Facing f in Facings)
            {
                // Cannot pass through walls
                if (data.DungeonCrawlerData.CurrentDungeon.IsPassable(p.Position, f))
                {
                    Vector2Int afterStep = p.Position.Step(f);
                    // Cannot move into space with enemy
                    bool isHero = figure.EntityData is HeroEntityData;
                    bool IsEnemy(EncounterFigureController otherFigure) => isHero ? otherFigure.Figure.EntityData is EnemyEntityData : otherFigure.Figure.EntityData is HeroEntityData;
                    if (state.Figures.TryGetValue(afterStep, out EncounterFigureController otherFigure) && IsEnemy(otherFigure))
                    {
                        continue;
                    }
                    yield return p with { Position = afterStep, Distance = distance, PreviousSpace = p };
                }
            }
        }
    }
}