using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed; 
    private Transform player;
    private Vector3 target;
    public GameObject poofEffect;
    public GameObject owner;
    Collider c;
    Rigidbody rb;
    Vector3 baseScale;
    Renderer m;
    public float upscaleSpeed = 0.001f;
    public float downscaleSpeed = 0.0005f;
    public AudioSource PoofSound;
    public AudioSource baseballHit;
    Vector3 launchDirection;
    bool ponged = false;
    bool popped = false;

    //For MEteor
    public string type="frost";
    public GameObject shadow;
    Vector3 shadowBaseScale;
    public AudioSource meteorSound;
    private void Awake()
    {
        m = GetComponent<Renderer>();
        c = GetComponent<Collider>();
        baseScale = transform.localScale;
        transform.localScale = Vector3.zero;
        rb = GetComponent<Rigidbody>();
        if (shadow != null)
        {
            shadowBaseScale = shadow.transform.localScale;
            shadow.transform.localScale = Vector3.zero;
        }
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
            if (shadow != null)
            {
                shadow.transform.localScale += modif/300;
            }
            yield return new WaitForFixedUpdate();
        }
        //yield return new WaitForSeconds(0.65f);
        launchDirection = new Vector3(player.position.x, player.position.y + 0.24f, player.position.z)*10000;
        c.enabled = true;
        if (type.Equals("fire"))
        {
            FallDown();
            
        }
        if (meteorSound != null)
        {
            meteorSound.Play();
        }
    }
    public IEnumerator FlyTowardsPlayer()
    {
        StartCoroutine(DownScale());
        float timeFlying = 0f;
        while(player!=null && timeFlying<4.5f&&!popped)
        {
            transform.Rotate(Vector3.forward *1f );
            target = new Vector3(player.position.x, player.position.y + 0.45f, player.position.z);

            transform.position = Vector3.MoveTowards(transform.position, target,speed);
            yield return new WaitForFixedUpdate();
            timeFlying += Time.deltaTime;
        }
        Pop();

    }
    public IEnumerator FlyTowardsPoint(Vector3 point)
    {
        target = transform.position - point;
        float timeFlying = 0f;
        string type = "up";
        switch (Random.Range(0, 2))
        {
            case 0: type = "up"; break;
            case 1: type = "right"; break;
                

        }
        transform.parent = null;
        while (player != null && !popped && timeFlying < 4.5f &&!ponged )
        {
            timeFlying += Time.deltaTime;
            if (type.Equals("up"))
            {
                transform.Rotate(Vector3.up * 5f);
                transform.position = Vector3.MoveTowards(transform.position, player.position + new Vector3(0f, 0.45f, 0f), speed * 1.4f);
                transform.Translate(target * speed * 0.4825f);
            }
            if (type.Equals("right"))
            {
                transform.Rotate(Vector3.right * 1f);
                transform.position = Vector3.MoveTowards(transform.position, player.position + new Vector3(0f, 0.55f, 0f), speed * 1.2f);
                transform.Translate(target * speed * 0.9f);
            }
            yield return new WaitForFixedUpdate();
        }
        if(timeFlying>=4.5f)
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
    IEnumerator Homerun()
    {
        owner = player.gameObject;
        Vector3 direction = player.transform.forward;
        transform.rotation = Quaternion.LookRotation(transform.position-player.forward);
        ponged = true;
        baseballHit.Play();
        while (!popped)
        {
            transform.Translate(direction*speed*2.4f);
            yield return new WaitForFixedUpdate();
        }
    }
    public void FallDown()
    {
        rb.isKinematic = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Ground")|| other.gameObject.CompareTag("LivingGround")|| other.gameObject.CompareTag("Wall"))
        {
            Pop();
        }
        if (other.gameObject.CompareTag("Enemy") &&!other.gameObject.Equals(owner))
        {
            other.GetComponent<Enemy>().Die();
        }
        if (other.gameObject.CompareTag("Player"))
        {
            //Musi to ist
            PlayerBehaviour pb = other.gameObject.GetComponent<PlayerBehaviour>();
            if (!pb.isAttacking || type.Equals("fire"))
            {
                pb.Die();
                Pop();
            }
            else
            {
                StartCoroutine(Homerun());
            }

        }
        if (other.gameObject.CompareTag("Bck"))
        {
            Pop();
        }
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
            if (type.Equals("fire"))
            {
                Destroy(transform.parent.gameObject, 0.4f);
            }
            popped = true;
        }
    }
}
