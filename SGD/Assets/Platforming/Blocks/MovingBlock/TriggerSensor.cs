using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSensor : MonoBehaviour
{
    public bool isNextToGround = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ground")|| other.gameObject.CompareTag("LivingGround") || other.gameObject.CompareTag("Wall"))
        {
            isNextToGround = true;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("LivingGround") || other.gameObject.CompareTag("Wall"))
        {
            isNextToGround = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("LivingGround") || other.gameObject.CompareTag("Wall"))
        {
            isNextToGround = false;
        }
    }
}
