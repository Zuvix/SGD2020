using System;
using System.Collections.Generic;
using System.ComponentModel;
using Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Management
{
    [Serializable]
    public class Blocks : SerializableDictionary<int, BlockDataObject> { }
    
    [Serializable]
    public class Placeables : SerializableDictionary<int, PlaceableDataObject> { }
    
    [Serializable]
    public class LoadScreenState
    {
        [Description("If false use default loading screen. Else load cinematic scene specified below.")] public bool loadCinematicScene;
        public int targetSceneBuildIndex;
        [Description("Lore or tip shown in bottom of the screen")] public string text = "";
    }
    
    [Serializable]
    public class DataManager : MonoBehaviour
    {
        public static DataManager instance;
        public Blocks blocks = new Blocks();
        public Placeables placeables = new Placeables();
        public List<string> defaultLoadTips = new List<string>();
        public List<LoadScreenState> screenPairs = new List<LoadScreenState>();
        
        // loadingPairIndex -=- buildIndex
        public List<Tuple<int, int>> StoryLevels = new List<Tuple<int, int>>()
        {
            new Tuple<int, int>(0, 4),
            new Tuple<int, int>(1, 5),
            new Tuple<int, int>(2, 6),
            new Tuple<int, int>(3, 7),
            new Tuple<int, int>(4, 8),
            new Tuple<int, int>(5, 1),
            new Tuple<int, int>(6, 15),
        };
        
        private void Awake()
        {
            if (instance == null || instance.Equals(null))
                instance = this;
            else
                Destroy(this);
        }
    }
}
