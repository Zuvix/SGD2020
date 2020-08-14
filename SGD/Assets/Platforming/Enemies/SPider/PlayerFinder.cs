using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFinder : MonoBehaviour
{
    bool found = false;
    SpiderBehaviour sb;
    private void Awake()
    {
        sb = GetComponentInParent<SpiderBehaviour>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") &&sb.target==null)
        {
            sb.AttackPlayer(other.gameObject.transform);
            found = true;
        }
    }
}
