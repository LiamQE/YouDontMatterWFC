using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;


[CreateAssetMenu(menuName = "WFC/TileData")]

public class TileData : ScriptableObject
{
    public string Name;
    public GameObject Prefab;

    public List<DirectionSocketPair> socketList;

    public Dictionary<Direction, string> socketDict;

    public void Initialize()
    {
        //toDictionary expects functions as input which is why lambda expression is used
        socketDict = socketList.ToDictionary(pair => pair.direction, pair => pair.socket); 
    }

    public string GetSocket(Direction dir)
    {
        if (socketDict == null) Initialize();
        return socketDict[dir];
    }

    public int weight = 1;
}

[System.Serializable]
public struct DirectionSocketPair
{
    public Direction direction;
    public string socket;
}

