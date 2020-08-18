using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemRotation : MonoBehaviour
{
    public GameObject gem;
    float speed = 50f;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * speed * Time.deltaTime, Space.World);
    }
}
