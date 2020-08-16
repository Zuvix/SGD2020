using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFinder : MonoBehaviour
{
    bool found = false;
    SpiderBehaviour sb;
    EyeSensor sen;
    private void Awake()
    {
        sb = GetComponentInParent<SpiderBehaviour>();
        sen = GetComponent<EyeSensor>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") &&sb.target==null &&sen.isOverGround)
        {
            sb.AttackPlayer(other.gameObject.transform);
            found = true;
        }
    }
}
