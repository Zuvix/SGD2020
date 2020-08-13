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
    public float steerIntensity = 0.38f;
    public float frontCheckIntensity = 0.15f;

    private EyeSensor front;
    private EyeSensor right;
    private EyeSensor left;
    private Vector3 lastPosition;
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
    public override void Activate()
    {
        StartCoroutine("BrainScope");
        Debug.Log("Enemy Activated");
    }
    public override void Die()
    {
        base.Die();
        squiqSound.Stop();
        StopAllCoroutines();
        impactSound.Play();
        //Destroy(this.gameObject);  //riesene pomocou Destroy(transform.parent.gameObject); v DissolveEffect.cs
        this.GetComponentInChildren<DissolveEffect>().startDissolve();
        //animacia
        
        Debug.Log("Spider killed, rip");
    }
    public override void Kill()
    {
        
    }

    // Update is called once per frame
    IEnumerator BrainScope()
    {
        StartCoroutine("TrapChecker");
        StartCoroutine("MovementLogic");
        yield return new WaitForFixedUpdate();
    } 
    IEnumerator TrapChecker()
    {
        while (true)
        {
            if (front.isEnemyAhead && front.timeCloseToEnemy > 0.15f)
            {
                Debug.Log("TRAP, WATCHOUT SPIDER");
                StopCoroutine("MovementLogic");
                anim.SetBool("isWalking", false);
                yield return new WaitUntil(() => front.isEnemyAhead == false);
                yield return new WaitForSeconds(0.2f);
                anim.SetBool("isWalking", true);
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
                yield return StartCoroutine(MoveAndLook());
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
        float timeTraveled = 0f;
        float timeROn=0f;
        float timeROff=0f;
        float timeLOn=0f;
        float timeLOff=0f;
        while (front.isOverGround || front.timeFromGround < frontCheckIntensity)
        {
            MoveSpider(1f);
            timeTraveled += Time.deltaTime;
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
    void MoveSpider(float modif)
    {
        lastPosition = new Vector3 (transform.position.x, transform.position.y, transform.position.z);
        transform.Translate(direction * speed*modif);

    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        Debug.DrawRay(transform.position, transform.forward, Color.blue);

    }

}
