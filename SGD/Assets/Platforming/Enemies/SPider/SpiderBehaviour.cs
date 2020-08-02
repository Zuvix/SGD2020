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

    public float steerIntensity = 0.38f;
    public float frontCheckIntensity = 0.15f;

    private EyeSensor front;
    private EyeSensor right;
    private EyeSensor left;
    private Vector3 lastPosition;
    public override void Awake()
    {
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
        Destroy(this.gameObject);
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
            if (front.isOverTrap && front.timeOverTrap > 0.15f)
            {
                Debug.Log("TRAP, WATCHOUT SPIDER");
                StopCoroutine("MovementLogic");
                yield return new WaitUntil(() => front.isOverTrap == false);
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
            
            if (left.isOverGround && left.timeOnGround > steerIntensity && timeLOff > timeLOn && !front.isOverTrap)
            {
                yield return StartCoroutine(Steer(false));
                break;
            }
            if (right.isOverGround && right.timeFromGround > steerIntensity && timeROff>timeROn && !front.isOverTrap)
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
    void FixedUpdate()
    {
        Debug.DrawRay(transform.position, transform.forward, Color.blue);

    }

}
