using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectGems : MonoBehaviour
{
    public PortalOpen portal;

    // Update is called once per frame
    void OnTriggerEnter(Collider other)
    {
        ScoringGemsSystem.gemsScore += 1;
        Destroy(transform.parent.gameObject);
        if (ScoringGemsSystem.gemsScore == ScoringGemsSystem.totalScore)
        {
            portal.open();
        }
    }
}
