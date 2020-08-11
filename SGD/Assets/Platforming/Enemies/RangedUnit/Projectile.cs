using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;

    private Transform player;
    private Vector3 target;
    public float lifeDuration = 3f;

    private float lifeTimer;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        target = new Vector3(player.position.x, player.position.y, player.position.z);
        lifeTimer = lifeDuration;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;

        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0f)
        {
            DestroyProjectile();
        }
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
