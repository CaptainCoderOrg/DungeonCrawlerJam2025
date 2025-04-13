using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter;
public abstract record class AttackTargetSelectedEvent;
public sealed record class ValidAttackTargetSelected(AttackInfo Attack) : AttackTargetSelectedEvent;
public sealed record class InvalidAttackTargetSelected(EncounterFigureController Target, string Reason) : AttackTargetSelectedEvent;
public sealed record class NoAttackTargetSelected : AttackTargetSelectedEvent
{
    public static readonly NoAttackTargetSelected Instance = new();
}

public sealed record class AttackInfo(Vector2Int StartPosition, Vector2Int TargetPosition, EncounterFigureController Target, int Distance);