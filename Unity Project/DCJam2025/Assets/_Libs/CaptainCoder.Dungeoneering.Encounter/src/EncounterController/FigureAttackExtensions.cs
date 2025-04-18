using System.Collections.Generic;
using System.Linq;

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


    /// <summary>
    /// Given a hero, an enemy that is moving, and the positions that that enemy will traverse, produces a GuardInfo of possible attack positions.
    /// </summary>
    /// <param name="hero"></param>
    /// <param name="target"></param>
    /// <param name="state"></param>
    /// <param name="encounterData"></param>
    /// <param name="positions"></param>
    /// <returns></returns>
    public static GuardInfo FindGuardPositions(this HeroFigurePanel hero, EncounterFigureController target, EncounterState state, EncounterData encounterData, IEnumerable<Vector2Int> positions)
    {
        HashSet<Vector2Int> positionLookup = positions.ToHashSet();
        HeroEntityData heroEntity = hero.FigureController.Figure.EntityData as HeroEntityData;
        AttackData attackData = PossibleAttacks(heroEntity).First();
        // Find positions that the enemy will move through
        AttackInfo attackInfo = hero.FigureController.Figure
                                            .FindAttackTargets(attackData, state, encounterData)
                                            .Where(info => positionLookup.Contains(info.TargetPosition))
                                            .OrderBy(a => a.Distance)
                                            .Select(s => s with { Target = target })
                                            .FirstOrDefault();
        return new GuardInfo(hero, attackData, target, attackInfo);
    }

    public static IEnumerable<AttackData> PossibleAttacks(this HeroEntityData hero)
    {
        if (hero.LeftHand != null && hero.LeftHand.Attack != null)
        {
            yield return hero.LeftHand.Attack;
        }

        if (hero.RightHand != null && hero.RightHand.Attack != null)
        {
            yield return hero.RightHand.Attack;
        }
        yield return hero.UnarmedAttack;
    }

    public static IEnumerable<DieData> CalculateAttackDice(this HeroEntityData attacker, AttackData attack)
    {
        yield return attack.AttackType.AttackDie;
        foreach (var die in attack.PowerDice)
        {
            yield return die;
        }
        foreach (var die in attacker.GetDice(attack))
        {
            yield return die;
        }
    }

}

public record class GuardInfo(HeroFigurePanel Hero, AttackData AttackData, EncounterFigureController Enemy, AttackInfo AttackInfo)
{
    public bool HasTargets() => AttackInfo != null;
    public FigureData Attacker() => Hero.FigureController.Figure;

    internal IEnumerable<DieData> AttackDice() => ((HeroEntityData)Hero.FigureController.Figure.EntityData).CalculateAttackDice(AttackData);
}