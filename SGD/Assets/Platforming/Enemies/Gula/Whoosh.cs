using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whoosh : MonoBehaviour
{
    public AudioSource whooshSound;
    public AudioSource gongSound;
    public void Whoossh()
    {
        whooshSound.Play();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Enemy"))
        {
            if(gongSound!=null)
                gongSound.Play();
        }
    }
}
