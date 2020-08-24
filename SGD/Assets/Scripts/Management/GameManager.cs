using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Data;
using IngameEditor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
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
        public KeyCode submit = KeyCode.Mouse0;
        public Material lineMat;

        [Header("UI")] 
        public Transform grid;
        public Transform selector;
        public Transform camera;
        public GameObject panel;
        public Transform shown;
        public TMP_Text mainCountdown;
        public TMP_Text blockCountdown;
        public TMP_Text blockCount;
        public RectTransform blockList; 
        
        [Header("Generation")] 
        public Transform blockStash;
        public GameObject blockObject;

        [Header("Editor")] 
        public float blockPlaceTime = 0.5f;
        public AnimationCurve blockPlaceCurve;
        public float blockArriveTime = 0.3f;
        public AnimationCurve blockArriveCurve;
        public float blockFallTime = 2f; 
        public AnimationCurve blockFallCurve;
        
        public Queue<GameObject> pool = new Queue<GameObject>();

        public GameObject selectedGameObject;

        //////////////////////////////////
        
        private float _timeSnapshot;
        private float _placingSnapshot;
        private int _poolIndex = 0;
        private PoolBlock _placing;
        private bool[,] _blocks;
        private bool _panelRevealed;
        private bool _roundEnded;
        private List<Tuple<Vector2Int, PoolBlock>> _locations = new List<Tuple<Vector2Int, PoolBlock>>(); 
        
        
        private void Awake()
        {
            if (instance == null || instance.Equals(null))
                instance = this;
            else
                Destroy(this);
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            _timeSnapshot = Time.time;
            mainCountdown.text = "";
            blockCountdown.text = "";
            blockCount.text = "";
        }

        private void Update()
        {
            if (Time.time > initialWaitTime + _timeSnapshot)
            {
                mainCountdown.text = "";
                blockCountdown.text = "";
                
                //Start
                if (_placing != null)
                {
                    if (!_panelRevealed)
                    {
                        panel.LeanMoveX(80, 0.5f);
                        _panelRevealed = true;
                    }
                    
                    selectedGameObject.transform.position = new Vector3(selector.GetChild(0).position.x, selectedGameObject.transform.position.y, selector.GetChild(0).position.z);
                    
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
                            
                        } while (_blocks[pos.x, pos.z]);
                        Place(new Vector3(pos.x, 0, pos.z));
                    }

                    // controls
                    if (selector.gameObject.activeSelf)
                    {
                        if (!_blocks[(int)(selector.position.x-0.5f), (int) (-selector.position.z-0.5f)])
                        {
                            selector.GetComponent<SpriteRenderer>().material.color = Color.green;
                            if (Input.GetMouseButtonDown(0))
                            {
                                Debug.Log("Place block");
                                
                                var position = selector.position;
                                Place(new Vector3(position.x-0.5f, 0, -position.z-0.5f));
                            }
                        }
                        else
                        {
                            selector.GetComponent<SpriteRenderer>().material.color = Color.red;
                        }
                    }

                    if (Input.GetKeyDown(rotateLeft))
                    {
                        selectedGameObject.transform.rotation = Quaternion.Euler(0,selectedGameObject.transform.rotation.eulerAngles.y+90,0);    
                    } else if (Input.GetKeyDown(rotateRight))
                    {
                        selectedGameObject.transform.rotation = Quaternion.Euler(0,selectedGameObject.transform.rotation.eulerAngles.y-90,0);    
                    }

                    
                    blockCountdown.text = Math.Ceiling(placeTime -(Time.time - _placingSnapshot)).ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    if (sourceData.levels[levelNum].blockPool.Count > 0 && sourceData.levels[levelNum].blockPool.Count > _poolIndex)
                    {
                        _placing = sourceData.levels[levelNum].blockPool[_poolIndex];
                        blockCount.text = $"{_poolIndex + 1}/{sourceData.levels[levelNum].blockPool.Count}";
                        selectedGameObject = pool.Dequeue();
                        selectedGameObject.GetComponent<MeshFilter>().mesh = _placing.poolBlockData.mesh;
                        selectedGameObject.GetComponent<MeshRenderer>().material = _placing.poolBlockData.material;
                        selectedGameObject.transform.position = selector.GetChild(0).position + Vector3.up;
                        for (var i = 0; i < 9; i++)
                        {
                            if (_placing.overridePlacings[i] != null)
                            {
                                // Check mesh
                                        
                                        
                                // Use icon
                                var icon = selectedGameObject.transform.GetChild(i);
                                icon.gameObject.SetActive(true);
                                var sr = icon.GetComponent<SpriteRenderer>();
                                sr.sprite = _placing.overridePlacings[i].icon;
                                sr.color = _placing.overridePlacings[i].iconColor;
                                icon.GetComponent<Icon>().cam = camera;

                                var uiSr = shown.GetChild(i).GetComponent<Image>();
                                uiSr.sprite = _placing.overridePlacings[i].icon;
                                uiSr.color = _placing.overridePlacings[i].iconColor;
                            }
                            else
                            {
                                var uiSr = shown.GetChild(i).GetComponent<Image>();
                                uiSr.color = Color.clear;
                            }
                        }
                        selectedGameObject.SetActive(true);
                        selectedGameObject.LeanMoveY(selector.GetChild(0).position.y, blockArriveTime).setEase(blockArriveCurve);
                        _placingSnapshot = Time.time;
                    }
                    else
                    {
                        // End placing turn
                        selector.gameObject.SetActive(false);
                        if (_panelRevealed)
                        {
                            panel.LeanMoveX(-80, 0.5f);
                            _panelRevealed = false;
                        }

                        if (!_roundEnded)
                        {
                            if (Input.GetKeyDown(submit))
                            {
                                for (var i = 0; i < blockStash.childCount; i++)
                                {
                                    var obj = blockStash.GetChild(i);
                                    obj.gameObject.LeanMoveY(-10, blockFallTime).setDelay(i / 5f).setEase(blockFallCurve);
                                }

                                StartCoroutine(ToGame());
                                _roundEnded = true;
                            }
                        }

                       
                        selectedGameObject = null;
                        _placing = null;
                    }
                }
            }
            else
            {
                mainCountdown.text = Math.Ceiling(initialWaitTime -(Time.time - _timeSnapshot)).ToString(CultureInfo.InvariantCulture);
            }
        }

        public void Place(Vector3 position)
        {
            selectedGameObject.LeanMove(new Vector3(position.x+0.5f, 0, -(position.z+0.5f)), blockPlaceTime).setEase(blockPlaceCurve);
            _blocks[(int) position.x, (int) position.z] = true;
            _locations.Add(new Tuple<Vector2Int, PoolBlock>(new Vector2Int((int) (position.x+0.5f), (int) -(position.z+0.5f)), _placing));
            
            // place on grid 
            _poolIndex++;
            _placing = null;
            selectedGameObject = null;
        }

        /*private void OnPostRender()
        {
            GL.Begin(GL.LINES);
            lineMat.SetPass(0);
            GL.Color(lineMat.color);
            for (var x = 0; x <= sourceData.levels[levelNum].dimensions.x; x++)
            {
                GL.Vertex3(x, 0, 0);
                GL.Vertex3(x, 0, sourceData.levels[levelNum].dimensions.y);
            }
            for (var y = 0; y <= sourceData.levels[levelNum].dimensions.y; y++)
            {
                GL.Vertex3(0, 0, y);
                GL.Vertex3(sourceData.levels[levelNum].dimensions.x, 0, y);
            }
            GL.End();
        }*/

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

        private IEnumerator ToGame()
        {
            yield return new WaitForSeconds(1.6f);
            TransitionManager.instance.PassData(levelNum, _locations);
        }
    }
}
