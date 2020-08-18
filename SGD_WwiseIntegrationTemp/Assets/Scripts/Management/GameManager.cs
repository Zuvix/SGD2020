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

        [Header("Game")] 
        public float initialWaitTime = 5f;
        
        
        [Header("UI")] 
        public Transform grid;
        public Transform selector;
        public Transform camera;
        
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

        private void Start()
        {
            // TODO: game start
        }

        public void Initialize(int index)
        {
            levelNum = index;

            if (levelNum >= 0 && sourceData.levels.Count >= levelNum + 1)
            {
                var level = sourceData.levels[levelNum];
                var sMover = grid.GetComponent<SelectorMover>();
                sMover.gridDimensions = level.dimensions;
                var size = 0;
                
                // Generate 
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
                                var obj = Instantiate(blockObject, new Vector3(0.5f + x, 0, -0.5f - y), Quaternion.Euler(0, ((int?) block.facing).Value * 90, 0), blockStash);
                                obj.GetComponent<MeshFilter>().mesh = block.layoutBlockData.mesh;
                                obj.GetComponent<MeshRenderer>().material = block.layoutBlockData.material;
                                for (var i = 0; i < 9; i++)
                                {
                                    if (block.overridePlacings[i] != null)
                                    {
                                        var icon = obj.transform.GetChild(i);
                                        icon.gameObject.SetActive(true);
                                        var sr = icon.GetComponent<SpriteRenderer>();
                                        sr.sprite = block.overridePlacings[i].icon;
                                        sr.color = block.overridePlacings[i].iconColor;
                                        icon.GetComponent<Icon>().cam = camera;
                                    }
                                }
                                size++;
                            }
                        }
                    }                    
                }
                
                // Populate object pools
                // TODO: object pooling x*y-count
            }
            else
            {
                throw new Exception("Tried to load nonexistent level data");
            }
        }
    }
}
