using System;
using System.Collections.Generic;
using System.Globalization;
using Data;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Management
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;
        public int levelNum;
        public LevelDataObject sourceData;

        [Header("Game")] 
        public bool manualRotation;
        public float initialWaitTime = 5f;
        public float placeTime = 5f;
        public float rotateTime = 1f;
        public KeyCode rotateLeft = KeyCode.X;
        public KeyCode rotateRight = KeyCode.C;

        [Header("UI")] 
        public Transform grid;
        public Transform selector;
        public Transform camera;
        public TMP_Text countdown;
        
        [Header("Generation")] 
        public Transform blockStash;
        public GameObject blockObject;

        public Queue<GameObject> pool = new Queue<GameObject>();

        public GameObject selectedGameObject;

        //////////////////////////////////
        
        private float _timeSnapshot;
        private float _placingSnapshot;
        private int _poolIndex = 0;
        private PoolBlock _placing;
        private bool[,] _blocks;
        
        
        private void Awake()
        {
            if (instance == null || instance.Equals(null))
                instance = this;
            else
                Destroy(this);
        }

        private void Start()
        {
            _timeSnapshot = Time.time;
        }

        private void Update()
        {
            if (Time.time > initialWaitTime + _timeSnapshot)
            {
                SetText("");
                //Start
                if (_placing != null)
                {
                    selectedGameObject.transform.position = selector.GetChild(0).position;
                    
                    if (Time.time > placeTime + +_placingSnapshot)
                    {
                        // Random place
                        Vector3Int pos;
                        do
                        {
                            pos = new Vector3Int(
                                Random.Range(0, sourceData.levels[levelNum].dimensions.x-1),
                                0,
                                Random.Range(0, sourceData.levels[levelNum].dimensions.y-1));
                            
                        } while (_blocks[pos.x, pos.y]);
                        Place(new Vector3(pos.x+0.5f, 0, pos.y+0.5f));
                    }

                    if (selector.gameObject.activeSelf)
                    {
                        if (!_blocks[(int) (selector.position.x - 0.5f), (int) -(selector.position.z + 0.5f)])
                        {
                            selector.GetComponent<MeshRenderer>().material.color = Color.green;
                            if (Input.GetMouseButtonDown(0))
                            {
                                Debug.Log("Place block");
                                Place(selector.position);
                            }
                        }
                        else
                        {
                            selector.GetComponent<MeshRenderer>().material.color = Color.red;
                        }
                    }

                    if (Input.GetKeyDown(rotateLeft))
                    {
                        selectedGameObject.transform.rotation = Quaternion.Euler(0,selectedGameObject.transform.rotation.y+90,0);    
                    } else if (Input.GetKeyDown(rotateRight))
                    {
                        selectedGameObject.transform.rotation = Quaternion.Euler(0,selectedGameObject.transform.rotation.y-90,0);    
                    }

                    // controls
                    SetText(Math.Ceiling(placeTime -(Time.time - _placingSnapshot)).ToString(CultureInfo.InvariantCulture));
                }
                else
                {
                    if (sourceData.levels[levelNum].blockPool.Count > 0 && sourceData.levels[levelNum].blockPool.Count > _poolIndex)
                    {
                        _placing = sourceData.levels[levelNum].blockPool[_poolIndex];
                        selectedGameObject = pool.Dequeue();
                        selectedGameObject.GetComponent<MeshFilter>().mesh = _placing.poolBlockData.mesh;
                        selectedGameObject.GetComponent<MeshRenderer>().material = _placing.poolBlockData.material;
                        selectedGameObject.SetActive(true);
                        _placingSnapshot = Time.time;
                    }
                    else
                    {
                        // End placing turn
                        selector.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                SetText(Math.Ceiling(initialWaitTime -(Time.time - _timeSnapshot)).ToString(CultureInfo.InvariantCulture));
            }
        }

        private void SetText(string text)
        {
            countdown.text = text;
        }
        
        public void Place(Vector3 position)
        {
            selectedGameObject.transform.position = new Vector3(position.x, 0, position.z);
            _blocks[(int) position.x, (int) position.z] = true;
            
            // place on grid 
            _poolIndex++;
            _placing = null;
            selectedGameObject = null;
        }

        public void Initialize(int index)
        {
            levelNum = index;
            _blocks = new bool[sourceData.levels[levelNum].dimensions.x, sourceData.levels[levelNum].dimensions.y];
            
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
                                        // Check mesh
                                        
                                        
                                        // Use icon
                                        var icon = obj.transform.GetChild(i);
                                        icon.gameObject.SetActive(true);
                                        var sr = icon.GetComponent<SpriteRenderer>();
                                        sr.sprite = block.overridePlacings[i].icon;
                                        sr.color = block.overridePlacings[i].iconColor;
                                        icon.GetComponent<Icon>().cam = camera;
                                    }
                                }
                                _blocks[x, y] = true;
                                size++;
                            }
                        }
                    }                    
                }
                
                // Populate object pools
                var levelPool = level.blockPool;
                for (var i = 0; i < levelPool.Count; i++)
                {
                    var a = Instantiate(blockObject, Vector3.zero, Quaternion.identity, blockStash);
                    a.SetActive(false);
                    pool.Enqueue(a);
                }
            }
            else
            {
                throw new Exception("Tried to load nonexistent level data");
            }
        }
    }
}
