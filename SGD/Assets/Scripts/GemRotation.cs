using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemRotation : MonoBehaviour
{
    public GameObject gem;
    float speed = 45f;
    float timeForDirection = 0f;
    bool up=true;
    public float upSpeed = 0.1f;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * speed * Time.deltaTime);
        if (up)
        {
            transform.Translate(Vector3.up * upSpeed);
            if (timeForDirection > 3f)
            {
                up = false;
                timeForDirection = 0;
            }
        }
        else
        {
            transform.Translate(Vector3.up * upSpeed * -1);
            if (timeForDirection > 3f)
            {
                up = true;
                timeForDirection = 0;
            }
        }
        timeForDirection += Time.deltaTime;
    }
}
