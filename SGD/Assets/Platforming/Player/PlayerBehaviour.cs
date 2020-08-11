using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBehaviour : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float jumpPower = 6f;
    public float turnSmoothTime = 0.1f;
    public bool doubleJump = true;
    public float accel = 50f;
    public float downForce = 10f;
    public float shadowDistanceFromGroud = 0.08f;

    public float leftGroundTime=0f;

    float turnSmoothVelocity;
    Transform cameraT;
    Rigidbody rb;
    Vector2 input;
    bool isJumping = false;
    public bool isOnGround;
    Animator anim;
    public LayerMask mask;
    public float maxRayDistance=1f;
    float jumpdelay=0f;
    public GameObject shadow;

    void Awake()
    {
        //animator = GetComponent<Animator>();
        mask = LayerMask.GetMask("Ground","Enemy");
        cameraT = Camera.main.transform;
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // RAycasting for landing
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        Physics.Raycast(ray, out hit, Mathf.Infinity, mask);
        shadow.transform.position = hit.point + Vector3.up * shadowDistanceFromGroud;
        //hit.distance;
        if (hit.distance <= maxRayDistance)
        {
                Debug.DrawRay(transform.position, Vector3.down * maxRayDistance, Color.green);
            if (rb.velocity.y < -0.2f)
            {
                anim.SetBool("isFalling", false);
                anim.SetBool("isLanding", true);
            }

        }
        else
        {
            Debug.DrawRay(transform.position, Vector3.down * maxRayDistance, Color.red);
        }
        if (isOnGround)
        {
            anim.SetBool("isFalling", false);
            anim.SetBool("isLanding", false);
        }
            // input
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        input = input.normalized;
        //Input.GetKeyDown(KeyCode.Jump) --> Input.GetButton("Jump")
        if (Input.GetButtonDown("Jump") && jumpdelay>0.1f &&(isOnGround||doubleJump))
        {
            isJumping = true;
        }
        jumpdelay += Time.deltaTime;
        if (input!=Vector2.zero)
        {
            anim.SetBool("isRunning", true);
        }
        else
        {
            anim.SetBool("isRunning", false);
        }
        if (isOnGround)
        {
            leftGroundTime = 0f;
        }
        else
        {
            leftGroundTime += Time.deltaTime;
        }
    }
    public void Die()
    {
        Destroy(this.gameObject);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (input != Vector2.zero)
        {
            float targetRotation = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
            //rb.velocity+=transform.forward*accel;
            transform.Translate(Vector3.forward * moveSpeed);
            //rb.velocity = new Vector3(Mathf.Clamp(rb.velocity.x, -moveSpeed, moveSpeed), rb.velocity.y, Mathf.Clamp(rb.velocity.z, -moveSpeed, moveSpeed));
        }
        if (isJumping && isOnGround)
        {
            rb.AddForce(Vector3.up * jumpPower);
            isJumping = false;
            jumpdelay = 0;
            anim.SetBool("isJumping", true);
            anim.SetBool("isFalling", false);
        }
        else if(isJumping && doubleJump)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpPower);
            doubleJump = false;
            isJumping = false;
            jumpdelay = 0;
            anim.SetBool("isJumping", true);
            anim.SetBool("isLanding", false);
            anim.Play("jump",0,0);
        }
        if(rb.velocity.y<-0.15f && !isOnGround)
        {
            rb.AddForce(Vector3.down * downForce, ForceMode.Force);
            if (rb.velocity.y < -0.25f &&leftGroundTime>0.2f)
                anim.SetBool("isFalling", true);
            anim.SetBool("isJumping", false);
        }

    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Lava"))
        {
            print("Padol si do lavy.");
            Destroy(gameObject);
        }
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (rb.velocity.y < -0.05f)
            {
                other.gameObject.GetComponent<Enemy>().Die();
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                rb.AddForce(Vector3.up * jumpPower);
                anim.SetBool("isJumping", true);
                anim.SetBool("isLanding", false);
                doubleJump = true;
            }
            else
            {
                Die();
            }
        }
        if (other.gameObject.CompareTag("Spike"))
        {
            Die();
        }
        if (other.gameObject.CompareTag("Projectile"))
        {
            Die();
        }
    }

}
