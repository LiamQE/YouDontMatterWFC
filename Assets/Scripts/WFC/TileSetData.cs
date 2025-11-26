#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections.Generic;
using UnityEngine;

//tile set data contains a list of all the tiles and the compatibility map

[CreateAssetMenu(menuName = "WFC/TileSet")] // makes a creatable object through rightclicking 
public class TileSetData : ScriptableObject
{
    public List<TileData> tiles;

    [System.Serializable]
    public class DirectionNeighbourPair
    {
        public Direction direction;
        public List<TileData> compatibleNeighbors;
    }

    [System.Serializable]
    public class CompatibilityEntry
    {
        public TileData tile;
        public List<DirectionNeighbourPair> neighbors = new(); //infers type from context, same as new List<DirectionNeighbourPair>()
    }

    public List<CompatibilityEntry> compatibilityMap = new();

    #if UNITY_EDITOR
    [ContextMenu("Build Compatibility Map")]
    public void BuildCompatibilityMap()
    {
        CompatibilityBuilder.Build(this);
        EditorUtility.SetDirty(this); // Save changes to the asset
        Debug.Log("Built compatibility map via context menu.");
    }
    #endif

}

