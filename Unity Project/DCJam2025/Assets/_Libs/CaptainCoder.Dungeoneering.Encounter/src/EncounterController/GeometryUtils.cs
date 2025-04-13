using System.Collections.Generic;
using System.Linq;

using CaptainCoder.Dungeoneering.DungeonMap;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter;
public static class GeometryUtils
{
    const float EdgeDelta = 0.500f;
    const float CenterOffsetDelta = 0.0001f; // We step slightly to the side to allow attacking around corners

    /// <summary>
    /// Get a list of line segments between all corners of two positions
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public static IEnumerable<LineSegment> CornersToCenter(this Vector2Int start, Vector2Int end)
    {
        foreach (var startCorner in start.Corners())
        {
            yield return new LineSegment(startCorner, end);
        }
    }

    public static IEnumerable<Vector2> Corners(this Vector2Int center)
    {
        yield return new(center.x - CenterOffsetDelta, center.y);
        yield return new(center.x + CenterOffsetDelta, center.y);
        yield return new(center.x, center.y - CenterOffsetDelta);
        yield return new(center.x, center.y + CenterOffsetDelta);
    }
    public static IEnumerable<Vector2Int> GetGridPositions(this LineSegment segment)
    {
        int minX = Mathf.FloorToInt(Mathf.Min(segment.Start.x, segment.End.x));
        int maxX = Mathf.FloorToInt(Mathf.Max(segment.Start.x, segment.End.x));
        int minY = Mathf.FloorToInt(Mathf.Min(segment.Start.y, segment.End.y));
        int maxY = Mathf.FloorToInt(Mathf.Max(segment.Start.y, segment.End.y));
        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                yield return new Vector2Int(x, y);
            }
        }
    }

    public static IEnumerable<LineSegment> GetWallSegments(this Dungeon dungeon, IEnumerable<Vector2Int> positions)
    {
        return positions
            .Select(p => (p, new Position(p.x, p.y)))
            .Select(pair => (pair.p, dungeon.GetTile(pair.Item2).Walls))
            .SelectMany(GetWallSegments);

        static IEnumerable<LineSegment> GetWallSegments((Vector2Int, TileWalls) pair)
        {
            (Vector2Int centerP, TileWalls walls) = pair;
            if (walls.North is WallType.Solid)
            {
                yield return new LineSegment(new Vector2(centerP.x - EdgeDelta, centerP.y - EdgeDelta),
                                             new Vector2(centerP.x + EdgeDelta, centerP.y - EdgeDelta));
            }
            if (walls.South is WallType.Solid)
            {
                yield return new LineSegment(new Vector2(centerP.x - EdgeDelta, centerP.y + EdgeDelta),
                                             new Vector2(centerP.x + EdgeDelta, centerP.y + EdgeDelta));
            }

            if (walls.East is WallType.Solid)
            {
                yield return new LineSegment(new Vector2(centerP.x + EdgeDelta, centerP.y - EdgeDelta),
                                             new Vector2(centerP.x + EdgeDelta, centerP.y + EdgeDelta));
            }

            if (walls.West is WallType.Solid)
            {
                yield return new LineSegment(new Vector2(centerP.x - EdgeDelta, centerP.y - EdgeDelta),
                                             new Vector2(centerP.x - EdgeDelta, centerP.y + EdgeDelta));
            }
        }
    }

    public static bool Intersects(this LineSegment a, LineSegment b)
    {
        int Orientation(Vector2 p, Vector2 q, Vector2 r)
        {
            float val = (q.y - p.y) * (r.x - q.x) -
                        (q.x - p.x) * (r.y - q.y);
            if (Mathf.Abs(val) == 0) return 0; // colinear
            return (val > 0) ? 1 : 2; // clockwise or counterclockwise
        }

        bool OnSegment(Vector2 p, Vector2 q, Vector2 r)
        {
            return Mathf.Min(p.x, q.x) <= r.x && r.x <= Mathf.Max(p.x, q.x) &&
                   Mathf.Min(p.y, q.y) <= r.y && r.y <= Mathf.Max(p.y, q.y);
        }

        Vector2 p1 = a.Start, p2 = a.End;
        Vector2 q1 = b.Start, q2 = b.End;

        int o1 = Orientation(p1, p2, q1);
        int o2 = Orientation(p1, p2, q2);
        int o3 = Orientation(q1, q2, p1);
        int o4 = Orientation(q1, q2, p2);

        if (o1 != o2 && o3 != o4) return true;

        if (o1 == 0 && OnSegment(p1, p2, q1)) return true;
        if (o2 == 0 && OnSegment(p1, p2, q2)) return true;
        if (o3 == 0 && OnSegment(q1, q2, p1)) return true;
        if (o4 == 0 && OnSegment(q1, q2, p2)) return true;

        return false;
    }

    public static bool IntersectsCircle(this LineSegment segment, Vector2Int center, float radius)
    {
        Vector2 d = segment.End - segment.Start;
        Vector2 f = segment.Start - center;

        float a = Vector2.Dot(d, d);
        float b = 2 * Vector2.Dot(f, d);
        float c = Vector2.Dot(f, f) - radius * radius;

        float discriminant = b * b - 4 * a * c;
        if (discriminant < 0)
        {
            // No intersection
            return false;
        }

        discriminant = Mathf.Sqrt(discriminant);

        float t1 = (-b - discriminant) / (2 * a);
        float t2 = (-b + discriminant) / (2 * a);

        // Check if either intersection point lies on the segment
        return (t1 >= 0 && t1 <= 1) || (t2 >= 0 && t2 <= 1);
    }

}