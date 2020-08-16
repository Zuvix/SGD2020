using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour
{
    public GameObject arrow;
    Vector3 arrowPos;
    Quaternion arrowRot;
    public float spawningTime=5f;
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
        while (true)
        {
            yield return new WaitForSeconds(spawningTime);
            arrow.SetActive(false);
            arrow.transform.position = arrowPos;
            arrow.transform.rotation = arrowRot;
            arrow.SetActive(true);

        }
    }


}
