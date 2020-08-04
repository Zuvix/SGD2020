using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectGems : MonoBehaviour
{

    // Update is called once per frame
    void OnTriggerEnter(Collider other)
    {
        ScoringGemsSystem.gemsScore += 1;
        Destroy(gameObject);        
    }
}
