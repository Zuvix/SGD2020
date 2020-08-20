using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using IngameEditor;
using Management;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : Singleton<LevelManager>
{
    public Text gemText;
    float gemCount=0;
    float maxGemCount = 3;
    public GameObject portal;
    public float respawnTime=5f;
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
    
    // Start is called before the first frame update
    void Start()
    {
        if (!isMenu)
        {
            maxGemCount = GameObject.FindGameObjectsWithTag("Gem").Length;
            gemText.text = " 0/" + maxGemCount;
            Player = GameObject.FindGameObjectWithTag("Player");
            StartCoroutine(RestartChecker());
        }
        yourTime = 0f;
        occupiedSpaces = new List<Vector3>();
    }
    private void Update()
    {
        yourTime += Time.deltaTime;
    }
    public void GetGem()
    {
        gemSound.Play();
        gemCount++;
        gemText.text = " "+gemCount+"/" + maxGemCount;
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
        if (_generated)
        {
            TransitionManager.instance.LoadLevel(_passedData.Item1+1);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
    public void RestartLevel()
    {
        if (_generated)
        {
            TransitionManager.instance.PassData(_passedData.Item1, _passedData.Item2);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
                case "f": Instantiate(frogPrefab, position, rotation); break;
                case "w": Instantiate(wizzardPrefab, position, rotation); break;

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
        if (index >= 0 && sourceData.levels.Count >= index + 1)
        {
            var level = sourceData.levels[index];
            var e = Math.Max(level.dimensions.x, level.dimensions.y) / 2f * 6f + 3f;
            env.position = new Vector3(e, 0, -e);
            
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
                                    var placing = Instantiate(block.overridePlacings[i].targetPrefab, obj.transform.position + Vector3.up*4,
                                        Quaternion.identity, spawn);
                                }
                            }
                        }
                    }
                }                    
            }

            foreach (var poolEntry in data)
            {
                var obj = Instantiate(poolEntry.Item2.poolBlockData.targetPrefab, new Vector3(3f + poolEntry.Item1.x*6, 0, -3f - poolEntry.Item1.y*6), Quaternion.Euler(0, 0, 0), spawn);
                for (var i = 0; i < 9; i++)
                {
                    if (poolEntry.Item2.overridePlacings[i] != null)
                    {
                        var placing = Instantiate(poolEntry.Item2.overridePlacings[i].targetPrefab, obj.transform.position+Vector3.up*4,
                            Quaternion.identity, spawn);
                    }
                }
            }

            _generated = true;
            _passedData = new Tuple<int, List<Tuple<Vector2Int, PoolBlock>>>(index, data);
        }
        else
        {
            throw new Exception("Tried to load nonexistent level data");
        }
    }
}
