﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
using Management;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelManager : Singleton<LevelManager>
{
    public TMP_Text gemText;
    float gemCount=0;
    float maxGemCount = 3;
    public GameObject portal;
    public float respawnTime=8f;
    public GameObject spiderPrefab;
    public GameObject frogPrefab;
    public GameObject wizzardPrefab;
    [HideInInspector]
    public GameObject Player;
    List<Vector3> occupiedSpaces;
    public AudioSource gemSound;
    public bool isMenu = false;

    //Stats Section
    public float avgTime=0f;
    public float yourTime=0f;
    public float bestTime = 0f;

    [Header("Generator")]
    public LevelDataObject sourceData;
    public Transform env;
    public Transform spawn;
    public CameraFollow camRef;
    private bool _generated;
    private Tuple<int, List<Tuple<Vector2Int, PoolBlock>>> _passedData;

    private bool _transitionOperation;
    private bool _hold;
    private float _snapshot;
    
    // Start is called before the first frame update
    void Start()
    {
        if (!isMenu)
        {
            maxGemCount = GameObject.FindGameObjectsWithTag("Gem").Length;
            gemText.text = "0/" + maxGemCount;
            Player = GameObject.FindGameObjectWithTag("Player");
            StartCoroutine(RestartChecker());
        }
        yourTime = 0f;
        occupiedSpaces = new List<Vector3>();
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.I))
        {
            yourTime += Time.deltaTime;
            if (yourTime > 1f)
            {
                FinishLevel();
            }
        yourTime += Time.deltaTime;
        if (!_transitionOperation)
        {
            if (Input.GetKey(KeyCode.P))
            {
                if (_hold)
                {
                    if (Time.time > _snapshot + 1.5f)
                    {
                        TransitionManager.instance.LoadScene(TransitionManager.SceneIndexes.Menu);
                        _transitionOperation = true;
                    }
                }
                else
                {
                    _snapshot = Time.time;
                }
            }

            _hold = Input.GetKey(KeyCode.P);
        }
    }
    public void GetGem()
    {
        gemSound.Play();
        gemCount++;
        gemText.text = gemCount+"/" + maxGemCount;
        if (gemCount >= maxGemCount)
        {
            gemText.text = "Max";
            portal.GetComponent<PortalOpener>().OpenPortal();
        }
    }
    public void ShowStats()
    {
        float finalTime = yourTime;
        //Player prefs when final
        float avgTime = 0;
        string yourMark = "A";

    }
    public void FinishLevel()
    {
        if (!_transitionOperation)
        {
            if (_generated)
            {
                if (sourceData.levels.Count > _passedData.Item1-1)
                    TransitionManager.instance.LoadLevel(_passedData.Item1+1);
                else
                    TransitionManager.instance.LoadScene(TransitionManager.SceneIndexes.Menu);
            }
            else
            {
                var currIndex = DataManager.instance.StoryLevels.IndexOf(
                    DataManager.instance.StoryLevels.FirstOrDefault(
                        x => x.Item2 == SceneManager.GetActiveScene().buildIndex));
                if (DataManager.instance.StoryLevels.Count - 1 > currIndex)
                    TransitionManager.instance.LoadStoryLevel(currIndex + 1, true);
                else
                    TransitionManager.instance.LoadScene(TransitionManager.SceneIndexes.Menu);
            }
            _transitionOperation = true;
        }
    }
    public void RestartLevel()
    {
        if (!_transitionOperation)
        {
            if (_generated)
            {
                TransitionManager.instance.PassData(_passedData.Item1, _passedData.Item2);
            }
            else
            {
                var currIndex = DataManager.instance.StoryLevels.IndexOf(
                    DataManager.instance.StoryLevels.FirstOrDefault(
                        x => x.Item2 == SceneManager.GetActiveScene().buildIndex));
                TransitionManager.instance.LoadStoryLevel(currIndex, false);
            }

            _transitionOperation = true;
        }
    }
    public IEnumerator SpawnMonster(string type, Vector3 position, Quaternion rotation)
    {
        bool correct = true;
        foreach (Vector3 space in occupiedSpaces)
        {
            if (position.Equals(space))
            {
                correct = false;
            }
        }
        if (correct)
        {
            occupiedSpaces.Add(position);
            yield return new WaitForSeconds(respawnTime);
            switch (type)
            {
                case "s": Instantiate(spiderPrefab, position, rotation); break;
                case "f": yield return new WaitForSeconds(respawnTime/2f); Instantiate(frogPrefab, position, rotation); break;
                case "w": yield return new WaitForSeconds(respawnTime+2f); Instantiate(wizzardPrefab, position, rotation); break;

                case "ff":

                    GameObject g = Instantiate(frogPrefab, position, rotation);
                    g.GetComponent<FrogBehaviour2>().Crown.SetActive(true);
                    break;
            }
        }
        yield return new WaitForFixedUpdate();
        occupiedSpaces.Remove(position);
        
    }
    IEnumerator RestartChecker()
    {
        yield return new WaitForSeconds(2f);
        while (true)
        {
            if (Player == null)
            {
                RestartLevel();
            }
            yield return new WaitForFixedUpdate();
        }

    }
    public void Initialize(int index, List<Tuple<Vector2Int, PoolBlock>> data)
    {
        _generated = true;
        _passedData = new Tuple<int, List<Tuple<Vector2Int, PoolBlock>>>(index, data);
        GameObject playerTemp = null;
        
        if (index >= 0 && sourceData.levels.Count >= index + 1)
        {
            var level = sourceData.levels[index];
            var e = Math.Max(level.dimensions.x, level.dimensions.y) / 2f * 6f + 3f;
            env.position = new Vector3(e, 0, -e);

            var generatedStart = false;
            var generatedFinish = false;
            
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
                            var obj = Instantiate(block.layoutBlockData.targetPrefab, new Vector3(3f + x*6, 0, -3 - y*6), Quaternion.Euler(0, ((int?) block.facing).Value * 90, 0), spawn);
                            for (var i = 0; i < 9; i++)
                            {
                                if (block.overridePlacings[i] != null)
                                {
                                    var placing = Instantiate(block.overridePlacings[i].targetPrefab, obj.transform);
                                    var a = new List<float>() {-1.8f, 0f, 1.8f};
                                    placing.transform.position = Vector3.zero;
                                    placing.transform.localPosition = new Vector3(a[i%3], 3, a[i/3]) / 300;
                                    placing.transform.localScale = placing.transform.localScale / 300;

                                    // Spawn
                                    if (block.overridePlacings[i].id == 2)
                                    {
                                        camRef.PlayerObj = placing;
                                        camRef.CameraFollowObj = placing.transform.GetChild(3).gameObject;
                                        placing.GetComponent<PlayerBehaviour>().cameraT = camRef.transform.GetChild(0);
                                        generatedStart = true;
                                        playerTemp = placing;
                                        playerTemp.SetActive(false);
                                    }
                                    // Portal
                                    if (block.overridePlacings[i].id == 1)
                                    {
                                        portal = placing;
                                        generatedFinish = true;
                                    }
                                    
                                    if (block.overridePlacings[i].id == 0)
                                    {
                                        placing.transform.localPosition += Vector3.up / 300;
                                        placing.transform.localScale = Vector3.one * 0.0005f;
                                        placing.GetComponent<CollectGems>().baseScale = placing.transform.localScale;
                                    }
                                    
                                    if (block.overridePlacings[i].id == 4)
                                    {
                                        var localPosition = placing.transform.localPosition;
                                        localPosition = new Vector3(localPosition.x, 2f/300,localPosition.z);
                                        placing.transform.localPosition = localPosition;
                                    }
                                }
                            }
                            
                            // If start not specified
                            if (level.startPos == new Vector2Int(x, y) && !generatedStart)
                            {
                                var placing = Instantiate(DataManager.instance.placeables[2].targetPrefab, obj.transform.position + Vector3.up*4,
                                    Quaternion.identity, obj.transform);
                                camRef.PlayerObj = placing;
                                camRef.CameraFollowObj = placing.transform.GetChild(3).gameObject;
                                generatedStart = true;
                                playerTemp = placing;
                                playerTemp.SetActive(false);
                            }
                            // If finish not specified
                            if (level.endPos == new Vector2Int(x, y) && !generatedFinish)
                            {
                                var placing = Instantiate(DataManager.instance.placeables[1].targetPrefab, obj.transform.position + Vector3.up*4,
                                    Quaternion.identity, obj.transform);
                                portal = placing;
                                generatedFinish = true;
                            }
                        }
                    }
                }
            }

            foreach (var poolEntry in data)
            {
                var obj = Instantiate(poolEntry.Item2.poolBlockData.targetPrefab,
                    new Vector3(3f + poolEntry.Item1.x * 6f, 0, -3f + poolEntry.Item1.y * 6f), Quaternion.Euler(0, 0, 0),
                    spawn);
                for (var i = 0; i < 9; i++)
                {
                    if (poolEntry.Item2.overridePlacings[i] != null)
                    {
                        var placing = Instantiate(poolEntry.Item2.overridePlacings[i].targetPrefab, obj.transform);
                        var a = new List<float>() {-1.8f, 0f, 1.8f};
                        placing.transform.localPosition = new Vector3(a[i%3], 3, a[i/3]) / 300;
                        placing.transform.localScale = placing.transform.localScale / 300;
                        
                        if (poolEntry.Item2.overridePlacings[i].id == 0)
                        {
                            placing.transform.localPosition += Vector3.up / 300;
                            placing.transform.localScale = Vector3.one * 0.0005f;
                            placing.GetComponent<CollectGems>().baseScale = placing.transform.localScale;
                        }
                        
                        if (poolEntry.Item2.overridePlacings[i].id == 4)
                        {
                            var localPosition = placing.transform.localPosition;
                            localPosition = new Vector3(localPosition.x, 2f/300,localPosition.z);
                            placing.transform.localPosition = localPosition;
                        }
                        
                    }
                }
            }
            
            if (playerTemp != null) playerTemp.SetActive(true);
        }
        else
        {
            throw new Exception("Tried to load nonexistent level data");
        }
    }
}
