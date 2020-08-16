using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour
{
    public GameObject arrow;
    Vector3 arrowPos;
    Quaternion arrowRot;
    public AudioSource shootSound;
    public AudioSource loadSound;
    private float spawningTime=4f;
    private void Awake()
    {
        arrowPos = arrow.transform.position;
        arrowRot = arrow.transform.rotation;
    }
    public void Activate()
    {
        StartCoroutine(Spawner());
    }
    private void Start()
    {
        Activate();
    }
    IEnumerator Spawner()
    {
        if (loadSound != null)
        {
            loadSound.Play();
        }
        yield return new WaitForSeconds(0.95f);
        if (shootSound != null)
        {
            shootSound.Play();
        }
        while (true)
        {
            yield return new WaitForSeconds(spawningTime);
            arrow.SetActive(false);
            arrow.transform.position = arrowPos;
            arrow.transform.rotation = arrowRot;
            arrow.SetActive(true);
            if (loadSound != null)
            {
                loadSound.Play();
            }
            yield return new WaitForSeconds(0.95f);
            if (shootSound != null)
            {
                shootSound.Play();
            }

        }
    }


}
