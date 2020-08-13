using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoringUIGemsSystem : MonoBehaviour
{
    public GameObject scoreText;
    //public GameObject gemImage;
    private void Update()
    {
        scoreText.GetComponent<Text>().text = "GEM SCORE: " + ScoringGemsSystem.gemsScore + "/" + ScoringGemsSystem.totalScore;
        //gemImage.transform.position = scoreText.transform.position;
    }
}
