using System;
using Data;
using UnityEngine;

namespace Management
{
    [Serializable]
    public class Blocks : SerializableDictionary<int, BlockDataObject> { }
    
    [Serializable]
    public class Placeables : SerializableDictionary<int, PlaceableDataObject> { }
    
    [Serializable]
    public class DataManager : MonoBehaviour
    {
        public static DataManager instance;
        public Blocks blocks = new Blocks();
        public Placeables placeables = new Placeables();
    }
}
