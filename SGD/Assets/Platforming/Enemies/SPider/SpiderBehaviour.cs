using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderBehaviour : MonoBehaviour
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
    void Awake()
    {
        front = farFrontEye.GetComponent<EyeSensor>();
        left = leftEye.GetComponent<EyeSensor>();
        right = rightEye.GetComponent<EyeSensor>();
    }
    private void Start()
    {
        StartCoroutine("BrainScope");
    }

    // Update is called once per frame
    IEnumerator BrainScope()
    {
        StartCoroutine("MovementLogic");
        yield return new WaitForFixedUpdate();
    } 
    IEnumerator MovementLogic()
    {
        yield return StartCoroutine(MoveForward());
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
    IEnumerator MoveForward()
    {
        while (front.isOverGround || front.timeFromGround < frontCheckIntensity)
        {
            MoveSpider(1f);
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
            MoveSpider(0.75f);
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
            
            if (left.isOverGround && left.timeOnGround > steerIntensity && timeLOff > timeLOn)
            {
                yield return StartCoroutine(Steer(false));
                break;
            }
            if (right.isOverGround && right.timeFromGround > steerIntensity && timeROff>timeROn)
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
