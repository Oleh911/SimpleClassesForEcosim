using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

public readonly struct Coordinates
{
    public readonly short X;
    public readonly short Y;

    public Coordinates(short x, short y)
    {
        X = x;
        Y = y;
    }

    public float DistanceTo(Coordinates coordinates)
    {
        return (float)Math.Sqrt(Math.Pow(X - coordinates.X, 2) + Math.Pow(Y - coordinates.Y, 2));
    }

    public Directions GetDirection(Coordinates to)
    {
        int dx = to.X - X;
        int dy = to.Y - Y;

        if (dx == 0)
            return dy > 0 ? Directions.Bottom : Directions.Top;

        if (dy == 0)
            return dx > 0 ? Directions.Right : Directions.Left;

        if (dx > 0 && dy > 0)
            return Directions.BottomRight;

        if (dx > 0 && dy < 0)
            return Directions.TopRight;

        if (dx < 0 && dy > 0)
            return Directions.BottomLeft;

        if (dx < 0 && dy < 0)
            return Directions.TopLeft;

        return Directions.None;
    }

    // потрібно додати радіус в параметри!
    public Coordinates OffsetByDirection(Directions direction)
    {
        return direction switch
        {
            Directions.Top => new Coordinates(X, (short)(Y - 1)),
            Directions.Bottom => new Coordinates(X, (short)(Y + 1)),
            Directions.Left => new Coordinates((short)(X + 1), Y),
            Directions.Right => new Coordinates((short)(X + 1), Y),
            Directions.TopLeft => new Coordinates((short)(X - 1), (short)(Y - 1)),
            Directions.TopRight => new Coordinates((short)(X + 1), (short)(Y - 1)),
            Directions.BottomLeft => new Coordinates((short)(X - 1), (short)(Y + 1)),
            Directions.BottomRight => new Coordinates((short)(X + 1), (short)(Y + 1)),
            _ => this
        };
    }

    public static List<Coordinates> GetOffsetsInRadius(int radius)
    {
        var offsets = new List<Coordinates>();

        for (int dx = -radius; dx <= radius; dx++)
        {
            for (int dy = -radius; dy <= radius; dy++)
            {
                offsets.Add(new Coordinates((short)dx, (short)dy));
            }
        }

        return offsets;
    }

    public static Coordinates operator +(Coordinates a, Coordinates b)
    {
        return new Coordinates((short)(a.X + b.X), (short)(a.Y + b.Y));
    }

    public static bool operator == (Coordinates left, Coordinates right)
    {
        if (left.X == right.X && left.Y == right.Y)
        {
            return true;
        }

        return false;
    }

    public static bool operator != (Coordinates left, Coordinates right)
    {
        if (left.X == right.X && left.Y == right.Y)
        {
            return false;
        }

        return true;
    }
}
