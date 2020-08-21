﻿using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed; 
    private Transform player;
    private Vector3 target;
    public GameObject poofEffect;
    Collider c;
    Rigidbody rb;
    Vector3 baseScale;
    Renderer m;
    public float upscaleSpeed = 0.001f;
    public float downscaleSpeed = 0.0005f;
    public AudioSource PoofSound;
    Vector3 launchDirection;
    bool popped = false;
    private void Awake()
    {
        m = GetComponent<Renderer>();
        c = GetComponent<Collider>();
        baseScale = transform.localScale;
        transform.localScale = Vector3.zero;
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        target = new Vector3(player.position.x, player.position.y + 0.24f, player.position.z);
        StartCoroutine(SpawnPojectile());
    }

    public IEnumerator SpawnPojectile()
    {
        c.enabled = false;
        while (transform.localScale.x < baseScale.x || transform.localScale.y < baseScale.y || transform.localScale.z < baseScale.z)
        {
            Vector3 modif = new Vector3(0, 0, 0);
            if (transform.localScale.x < baseScale.x)
            {
                modif.x += upscaleSpeed;
            }
            if (transform.localScale.y < baseScale.y)
            {
                modif.y += upscaleSpeed;
            }
            if (transform.localScale.z < baseScale.z)
            {
                modif.z += upscaleSpeed;
            }
            transform.localScale += modif;
            yield return new WaitForFixedUpdate();
        }
        //yield return new WaitForSeconds(0.65f);
        launchDirection = new Vector3(player.position.x, player.position.y + 0.24f, player.position.z)*10000;
        c.enabled = true;
    }
    public IEnumerator FlyTowardsPlayer()
    {
        StartCoroutine(DownScale());
        float timeFlying = 0f;
        while(player!=null && timeFlying<4.5f&&!popped)
        {
            transform.Rotate(Vector3.right *4f );
            target = new Vector3(player.position.x, player.position.y + 0.45f, player.position.z);

            transform.position = Vector3.MoveTowards(transform.position, target,speed);
            yield return new WaitForFixedUpdate();
            timeFlying += Time.deltaTime;
        }
        Pop();

    }
    IEnumerator DownScale()
    {
        while (transform.localScale.x > 0 || transform.localScale.y >0 || transform.localScale.z >0)
        {
            Vector3 modif = new Vector3(0, 0, 0);
            if (transform.localScale.x > 0)
            {
                modif.x -= downscaleSpeed;
            }
            if (transform.localScale.y > 0)
            {
                modif.y -= downscaleSpeed;
            }
            if (transform.localScale.z > 0)
            {
                modif.z -= downscaleSpeed;
            }
            transform.localScale += modif;
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForFixedUpdate();
    }

    public void Pop()
    {
        if (!popped)
        {
            StopAllCoroutines();
            c.enabled = false;
            m.enabled = false;
            PoofSound.Play();
            GameObject g = Instantiate(poofEffect);
            g.transform.position = transform.position;
            Destroy(this.gameObject, 0.4f);
            popped = true;
        }
    }
}
