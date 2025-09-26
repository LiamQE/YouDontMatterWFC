using UnityEngine;
using System.Collections.Generic;


public static class CompatibilityBuilder
{
    public static void Build(TileSetData tileSet)
    {
        tileSet.compatibilityMap.Clear();

        foreach (var tile in tileSet.tiles)
        {
        tile.Initialize();
        }

        foreach (var tile in tileSet.tiles)
        {
            TileSetData.CompatibilityEntry entry = new();
            entry.tile = tile;
            entry.neighbors = new();

            foreach (Direction direction in System.Enum.GetValues(typeof(Direction)))
            {
                List<TileData> compatible = new();

                foreach (var otherTile in tileSet.tiles)
                {
                    if (MatchingSockets(tile.socketDict[direction], otherTile.socketDict[GetOppositeDir(direction)]))
                    {
                        compatible.Add(otherTile);
                    }
                }

                entry.neighbors[direction] = compatible;
            }
            tileSet.compatibilityMap.Add(entry);
        }
        Debug.Log("compatibility map built.");

    }

    public static Direction GetOppositeDir(Direction dir)
    {
        Direction oppositeDirection = Direction.Up;
        if (dir == Direction.Up){oppositeDirection = Direction.Down;}
        else if (dir == Direction.Down){oppositeDirection = Direction.Up;}
        else if (dir == Direction.Left){oppositeDirection = Direction.Right;}
        else if (dir == Direction.Right){oppositeDirection = Direction.Left;}

        return oppositeDirection;
    }

    public static bool MatchingSockets(string A, string B)
    {
        if (A == B) {return true;}
        else {return false;}
    }
}