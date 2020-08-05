using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBlock : MonoBehaviour
{
    public GameObject Front;
    public GameObject Back;
    public float WaitTime=2f;
    public float flyspeed=1f;
    public float maxX=30;
    public float minX=-30;
    public float maxZ = 30;
    public float minZ = -30;
    bool frontBound = false;
    bool backBound = false;

    public Material mat;
    TriggerSensor fs;
    TriggerSensor bs;
    private MeshRenderer meshRenderer;


    void Awake()
    {
        fs = Front.GetComponent<TriggerSensor>();
        bs = Back.GetComponent<TriggerSensor>();
        FixRotation();
        meshRenderer = GetComponent<MeshRenderer>();
        StartCoroutine("FlyBaby");


    }
    public void FixRotation()
    {
        float y = transform.rotation.y;
        y = Mathf.Abs(y);
        y %= 360;

        //transform.rotation = new Quaternion(transform.rotation.x, y, transform.rotation.z, transform.rotation.w);
        
        
    }
    public void SetEmmision(bool toEmit)
    {
        if(toEmit)
            meshRenderer.material.SetColor("_EmissionColor", Color.white*2.5f);
        else
            meshRenderer.material.SetColor("_EmissionColor", Color.white);
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
            //Debug.Log("bg"+bg+" fg"+fg);
            yield return new WaitForFixedUpdate();
        }

    }
    IEnumerator Flying(string direction)
    {
        SetEmmision(true);
        if (direction.Equals("front"))
        {
            while (!fs.isNextToGround)
            {
                transform.Translate(Vector3.forward * flyspeed);
                yield return new WaitForFixedUpdate();
            }
        }
        else
        {
            while (!bs.isNextToGround)
            {
                transform.Translate(Vector3.forward*-1 * flyspeed);
                yield return new WaitForFixedUpdate();
            }
        }
        SetEmmision(false);
        yield return new WaitForSeconds(WaitTime);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.SetParent(transform);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.parent = null;
        }
    }
}
