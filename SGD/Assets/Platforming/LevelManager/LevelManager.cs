using System.Collections;
using System.Collections.Generic;
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

    // Start is called before the first frame update
    void Start()
    {
        maxGemCount = GameObject.FindGameObjectsWithTag("Gem").Length;
        gemText.text = " 0/" + maxGemCount;
    }
    public void GetGem()
    {
        gemCount++;
        gemText.text = " "+gemCount+"/" + maxGemCount;
        if (gemCount >= maxGemCount)
        {
            gemText.text = "Max";
            portal.GetComponent<PortalOpener>().OpenPortal();
        }
    }
    public void FinishLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public IEnumerator SpawnMonster(string type, Vector3 position, Quaternion rotation)
    {
        yield return new WaitForSeconds(respawnTime);
        switch (type)
        {
            case "s": Instantiate(spiderPrefab, position, rotation); break;
            case "f": Instantiate(frogPrefab, position, rotation); break;
            case "w": Instantiate(wizzardPrefab, position, rotation); break;
        }
        
    }
}
