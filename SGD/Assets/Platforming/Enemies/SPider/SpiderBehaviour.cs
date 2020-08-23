using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderBehaviour : Enemy
{
    // Start is called before the first frame update
    public GameObject farFrontEye;
    public GameObject leftEye;
    public GameObject rightEye;
    public float speed=0.05f;
    public float rotateSpeed = 20f;
    private Vector3 direction=Vector3.forward;

    public AudioSource impactSound;
    public AudioSource squiqSound;
    public AudioSource attackSound;
    public float steerIntensity = 0.38f;
    public float frontCheckIntensity = 0.15f;
    private EyeSensor front;
    private EyeSensor right;
    private EyeSensor left;
    public Transform target=null;
    public GameObject effect;
    private bool effectUnused=true;
    //public GameObject dissolve;
    public override void Awake()
    {
        base.Awake();
        front = farFrontEye.GetComponent<EyeSensor>();
        left = leftEye.GetComponent<EyeSensor>();
        right = rightEye.GetComponent<EyeSensor>();
    }
    private void Start()
    {
        Activate();
    }
    public override void Die()
    {
        base.Die();
        LevelManager.Instance.StartCoroutine(LevelManager.Instance.SpawnMonster("s", startPos, startRot));
        squiqSound.Stop();
        StopAllCoroutines();
        impactSound.Play();
        speed = 0f;
        //Destroy(this.gameObject);  //riesene pomocou Destroy(transform.parent.gameObject); v DissolveEffect.cs
        this.GetComponentInChildren<DissolveEffect>().StartDissolve();
        //animacia
        
        Debug.Log("Spider killed, rip");
    }

    // Update is called once per frame
    IEnumerator BrainScope()
    { 
        //yield return new WaitForSeconds(0.25f);
        anim.speed = 1f;
        StartCoroutine("TrapChecker");
        StartCoroutine("MovementLogic");
        yield return new WaitForSeconds(0.25f);
    } 
    IEnumerator TrapChecker()
    {
        while (true)
        {
            if (front.isEnemyAhead && front.timeCloseToEnemy > 0.15f)
            {
                Debug.Log("TRAP, WATCHOUT SPIDER");
                StopCoroutine("MovementLogic");
                anim.speed = 0f;
                if (effect != null)
                {
                    if (effectUnused)
                    {
                        Debug.Log("effect");
                        Instantiate(effect, transform.position+Vector3.up*0.3f, effect.transform.rotation);
                        effectUnused = false;
                    }
                }
                yield return new WaitUntil(() => front.isEnemyAhead == false);
                yield return new WaitForSeconds(0.2f);
                anim.speed = 1f;
                StartCoroutine("MovementLogic");
            }
            yield return new WaitForFixedUpdate();
        }
    }
    IEnumerator MovementLogic()
    {
        while (true)
        {
            bool l = left.isOverGround;
            bool r = right.isOverGround;
            bool f = front.isOverGround;
            if (f && l && r)
            {
                yield return StartCoroutine(MoveForward());
                Debug.Log("Moving forw");
            }
            else if ((!f && l && r) || (!f && !l && r))
            {
                yield return StartCoroutine(Steer(true));
                yield return StartCoroutine(MoveAndLook());
            }
            else if (!f && l && !r)
            {
                yield return StartCoroutine(Steer(false));
                yield return StartCoroutine(MoveAndLook());
            }
            else if(!f && !l && !r)
            {
                yield return StartCoroutine(Backward());
            }
            else
            {
                yield return StartCoroutine(MoveAndLook());
            }
            yield return new WaitForFixedUpdate();

        }
        
    }
    IEnumerator Steer(bool toRight)
    { 
        float startYRot = transform.rotation.y;
        float diff = 0;
        float rotateDirection;
        if (toRight)
        {
            rotateDirection = 1;
        }
        else
        {
            rotateDirection = -1;
        }

        while (diff < 90)
        {
            transform.Rotate(Vector3.up, rotateSpeed*rotateDirection);
            diff += rotateSpeed;
            MoveSpider(0.65f);
            yield return new WaitForFixedUpdate();
        }
    }
    IEnumerator MoveAndLook()
    {
        float timeROn=0f;
        float timeROff=0f;
        float timeLOn=0f;
        float timeLOff=0f;
        while (front.isOverGround || front.timeFromGround < frontCheckIntensity)
        {
            MoveSpider(1f);
            yield return new WaitForFixedUpdate();
            
            if (left.isOverGround && left.timeOnGround > steerIntensity && timeLOff > timeLOn && !front.isEnemyAhead)
            {
                yield return StartCoroutine(Steer(false));
                break;
            }
            if (right.isOverGround && right.timeFromGround > steerIntensity && timeROff>timeROn && !front.isEnemyAhead)
            {
                yield return StartCoroutine(Steer(true));
                break;
            }
            if (right.isOverGround)
            {
                timeROn += Time.deltaTime;
            }
            else
            {
                timeROff += Time.deltaTime;
            }
            if (left.isOverGround)
            {
                timeLOn += Time.deltaTime;
            }
            else
            {
                timeLOff += Time.deltaTime;
            }

        }
    }
    IEnumerator MoveForward()
    {
        while (front.isOverGround || front.timeFromGround < frontCheckIntensity)
        {
            transform.Translate(Vector3.forward * speed);
            yield return new WaitForFixedUpdate();
        } 
    }
    IEnumerator Backward()
    {
        anim.speed = 0.4f;
        while (front.timeOnGround<0.1f && right.timeOnGround<0.1f && left.timeOnGround < 0.1f && isOnGround)
        {
            transform.Translate(Vector3.forward * speed*-0.4f);
            yield return new WaitForFixedUpdate();
        }
        anim.speed = 1f;
    }
    IEnumerator KillPlayer()
    {
        while (true)
        {
            Vector3 playerLoc = new Vector3(target.position.x, transform.position.y, target.position.z);
            if(Vector3.Distance(transform.position,playerLoc)>0.5f)
                transform.rotation = Quaternion.LookRotation(playerLoc - transform.position);
            if (isOnGround)
                MoveSpider(1.25f);
            else
                MoveSpider(0.5f);
            if (front.timeFromGround > 0.225f)
            {
                AbandonTarget();
            }
            yield return new WaitForFixedUpdate();
        }

    }
    void MoveSpider(float modif)
    {
        transform.Translate(direction * speed*modif);
    }
    public void AttackPlayer(Transform player)
    {
        if (summoningComplete)
        {
            StopAllCoroutines();
            attackSound.Play();
            target = player;
            StartCoroutine("KillPlayer");
            anim.speed = 1.25f;
        }
    }
    public void AbandonTarget()
    {
        StopCoroutine("KillPlayer");
        anim.speed = 1f;
        target = null;
        StartCoroutine("BrainScope");

    }
    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (other.gameObject.CompareTag("Player"))
        {
            StopAllCoroutines();
            anim.speed = 0.2f;
        }
        if (other.gameObject.CompareTag("Void"))
        {
            Die();
        }
    }

}
