using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;

    private Transform player;
    private Vector3 target;
    Collider c;
    Vector3 baseScale;
    public float upscaleSpeed = 0.001f;
    public float downscaleSpeed = 0.0005f;
    private void Awake()
    {
        c = GetComponent<Collider>();
        baseScale = transform.localScale;
        transform.localScale = Vector3.zero;
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        target = new Vector3(player.position.x, player.position.y + 0.1f, player.position.z);
        StartCoroutine(SpawnPojectile());
    }

    public IEnumerator SpawnPojectile()
    {
        c.enabled = false;
        while (transform.localScale.x < baseScale.x && transform.localScale.y < baseScale.y && transform.localScale.z < baseScale.z)
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
        c.enabled = true;
        StartCoroutine(FlyTowardsPlayer());
    }
    IEnumerator FlyTowardsPlayer()
    {
        StartCoroutine("DownScale");
        while(player!=null && transform.localScale.magnitude > 0f)
        {
            target = new Vector3(player.position.x, player.position.y + 0.25f, player.position.z);
            transform.position=Vector3.MoveTowards(transform.position, target, speed);
            yield return new WaitForFixedUpdate();
        }

    }
    IEnumerable DownScale()
    {
        while (transform.localScale.x < baseScale.x && transform.localScale.y < baseScale.y && transform.localScale.z < baseScale.z)
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
        yield return new WaitForFixedUpdate();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            print("Umrel si.");
        }
        DestroyProjectile();
    }
    void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}
