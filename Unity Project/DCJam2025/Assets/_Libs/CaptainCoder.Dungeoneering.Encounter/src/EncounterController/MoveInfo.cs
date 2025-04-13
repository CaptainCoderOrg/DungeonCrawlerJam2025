using System.Collections.Generic;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter;
public record class MoveInfo(Vector2Int Position, MoveInfo PreviousSpace, int Distance)
{
    public IEnumerable<Vector2Int> Path()
    {
        MoveInfo current = this;
        while (current != null)
        {
            yield return current.Position;
            current = current.PreviousSpace;
        }
    }
}