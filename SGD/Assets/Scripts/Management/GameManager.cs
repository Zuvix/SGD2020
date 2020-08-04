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

        [Header("Generation")] 
        public Transform blockStash;
        public GameObject blockObject;
        
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

                for (var x = 0; x < level.dimensions.x; x++)
                {
                    for (var y = 0; y < level.dimensions.y; y++)
                    {
                        if (level.layout.ContainsKey(x))
                        {
                            if (level.layout[x].ContainsKey(y))
                            {
                                // Generate block primitive
                                var block = level.layout[x][y];
                                var obj = Instantiate(blockObject, new Vector3(0.5f + x, 0, -0.5f - y), Quaternion.identity, blockStash);
                                obj.GetComponent<MeshFilter>().mesh = block.layoutBlockData.mesh;
                                obj.GetComponent<MeshRenderer>().material = block.layoutBlockData.material;
                            }
                        }
                    }                    
                }
            }
            else
            {
                throw new Exception("Tried to load nonexistent level data");
            }
        }
    }
}
