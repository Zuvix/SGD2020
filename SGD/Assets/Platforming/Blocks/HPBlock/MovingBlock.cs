using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBlock : MonoBehaviour
{
    public GameObject Front;
    public GameObject Back;
    public float WaitTime=2f;
    public float flyspeed=1f;

    TriggerSensor fs;
    TriggerSensor bs;
    void Awake()
    {
        fs = Front.GetComponent<TriggerSensor>();
        bs = Back.GetComponent<TriggerSensor>();
        StartCoroutine("FlyBaby");

    }
    IEnumerator FlyBaby()
    {
        while (true)
        {
            bool fg = fs.isNextToGround;
            bool bg = bs.isNextToGround;
            if (!fg && !bg)
            {
                yield return StartCoroutine(Flying("back"));
            }
            if (!fg && bg)
            {
                yield return StartCoroutine(Flying("front"));
            }
            if(fg && !bg)
            {
                yield return StartCoroutine(Flying("back"));
            }
            if(bg && fg)
            {             
                yield return new WaitForSeconds(1f);
            }
            Debug.Log("bg"+bg+" fg"+fg);
            yield return new WaitForFixedUpdate();
        }

    }
    IEnumerator Flying(string direction)
    {
        if (direction.Equals("front"))
        {
            while (!fs.isNextToGround)
            {
                transform.Translate(Vector3.right* flyspeed);
                yield return new WaitForFixedUpdate();
            }
        }
        else
        {
            while (!bs.isNextToGround)
            {
                transform.Translate(Vector3.left * flyspeed);
                yield return new WaitForFixedUpdate();
            }
        }
        yield return new WaitForSeconds(WaitTime);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.transform.SetParent(transform);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.transform.parent = null;
        }
    }
}
