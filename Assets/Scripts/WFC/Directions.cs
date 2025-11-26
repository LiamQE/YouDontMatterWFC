using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    Up, Down, Left, Right
}

public static class DirectionUtil
{
    public static readonly Dictionary<Direction, (int dx, int dy)> DirectionVec = 
        new() 
        { 
            { Direction.Up,    (0, -1) },
            { Direction.Down,  (0,  1) },
            { Direction.Left,  (-1, 0) },
            { Direction.Right, (1,  0) }
        };
}