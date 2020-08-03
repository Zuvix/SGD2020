using System;
using System.Collections;
using Data;
using UnityEngine;
using UnityFx.Async;

namespace Management
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;
        public int levelNum;
        public LevelDataObject sourceData;

        [Header("UI")] 
        public Transform grid;
        public Transform selector;

        private void Awake()
        {
            if (instance == null || instance.Equals(null))
                instance = this;
            else
                Destroy(this);
        }

        public void Initialize(int index)
        {
            levelNum = index;

            if (levelNum >= 0 && sourceData.levels.Count >= levelNum + 1)
            {
                var level = sourceData.levels[levelNum];
                var sMover = grid.GetComponent<SelectorMover>();
                sMover.gridDimensions = level.dimensions;
            }
            else
            {
                throw new Exception("Tried to load nonexistent level data");
            }
        }
    }
}
