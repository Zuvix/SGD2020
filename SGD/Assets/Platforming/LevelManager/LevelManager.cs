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
    [HideInInspector]
    public GameObject Player;
    List<Vector3> occupiedSpaces;
    public AudioSource gemSound;

    //Stats Section
    public float avgTime=0f;
    public float yourTime=0f;
    public float bestTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        maxGemCount = GameObject.FindGameObjectsWithTag("Gem").Length;
        gemText.text = " 0/" + maxGemCount;
        Player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(RestartChecker());
        occupiedSpaces = new List<Vector3>();
        yourTime = 0f;   
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
}
