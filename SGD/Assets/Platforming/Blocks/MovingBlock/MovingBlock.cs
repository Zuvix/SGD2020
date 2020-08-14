using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBlock : MonoBehaviour
{
    public GameObject Front;
    public GameObject Back;
    public float WaitTime=2f;
    public float flyspeed=1f;
    public float maxDistance;
    float currentDistance=0f;
    bool frontBound = false;
    bool backBound = false;
    public AudioSource moveSound;

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
        transform.rotation = new Quaternion(transform.rotation.x, y, transform.rotation.z, transform.rotation.w);
        
        
    }
    public void SetEmmision(bool toEmit)
    {
        if(toEmit)
            meshRenderer.material.SetColor("_EmissionColor", Color.white);
        else
            meshRenderer.material.SetColor("_EmissionColor", Color.black);
    }
    IEnumerator FlyBaby()
    {
        while (true)
        {
            bool fg = fs.isNextToGround;
            bool bg = bs.isNextToGround;
            if (!fg && !bg && currentDistance<=maxDistance && currentDistance>=-maxDistance)
            {
                this.gameObject.layer = 14;
                moveSound.Play();
                yield return StartCoroutine(Flying("back"));
            }
            if (!fg && (bg || currentDistance <= -maxDistance))
            {
                this.gameObject.layer = 14;
                moveSound.Play();
                yield return StartCoroutine(Flying("front"));
            }
            if((fg || currentDistance>=maxDistance) && !bg)
            {
                this.gameObject.layer = 14;
                moveSound.Play();
                yield return StartCoroutine(Flying("back"));
            }
            if(bg && fg)
            {
                this.gameObject.layer = 8;
                moveSound.Stop();
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
            while (!fs.isNextToGround && currentDistance<=maxDistance)
            {
                transform.Translate(Vector3.left * flyspeed);
                currentDistance += flyspeed;
                yield return new WaitForFixedUpdate();
            }
        }
        else
        {
            while (!bs.isNextToGround && currentDistance>= -maxDistance)
            {
                transform.Translate(Vector3.left*-1 * flyspeed);
                currentDistance -= flyspeed;
                yield return new WaitForFixedUpdate();
            }
        }
        SetEmmision(false);
        this.gameObject.layer = 8;
        moveSound.Stop();
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
