using System;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class LayoutBlockDict : SerializableDictionary<int, LayoutBlock> { }
    
    [Serializable]
    public class LbdWrapper : SerializableDictionary<int, LayoutBlockDict> { }
    
    [Serializable]
    public enum Rotation
    {
        NORTH = 0,
        EAST= 1,
        SOUTH = 2,
        WEST= 3
    }
    
    [Serializable]
    public class PoolBlock
    {
        public BlockDataObject poolBlockData;
        public int count = 1;
        [Range(0f, 1f)]
        public float chance = 1f;
        public PlaceableDataObject[] overridePlacings = new PlaceableDataObject[9];
    }

    [Serializable]
    public class LayoutBlock
    {
        public BlockDataObject layoutBlockData;
        public Rotation facing;
        public PlaceableDataObject[] overridePlacings;
    }
    
    [Serializable]
    public class LevelData
    {
        public Vector2Int dimensions;
        public Vector2Int startPos = new Vector2Int(-1, -1);
        public Vector2Int endPos = new Vector2Int(-1, -1);
        // Layout 
        public LbdWrapper layout = new LbdWrapper();
        // Pool
        public bool ordered;
        public List<PoolBlock> blockPool = new List<PoolBlock>();
    }
}

