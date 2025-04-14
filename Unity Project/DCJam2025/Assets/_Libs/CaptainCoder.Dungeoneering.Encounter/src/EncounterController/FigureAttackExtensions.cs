using System.Collections.Generic;

using CaptainCoder.Dungeoneering.DungeonMap;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter;

public static class FigureAttackExtensions
{
    private static readonly Vector2Int[] Directions = new Vector2Int[]
        {
            new(-1, -1), new( 0, -1), new( 1, -1),
            new(-1,  0),              new( 1,  0),
            new(-1,  1), new( 0,  1), new( 1,  1),
        };
    internal static HashSet<AttackInfo> FindAttackTargets(this FigureData attacker, AttackData attackData, EncounterState state, EncounterData data)
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
                if (!data.IsInBounds(afterStep)) { continue; }
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

}