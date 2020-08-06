using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float jumpPower = 6f;
    public float turnSmoothTime = 0.1f;
    public bool doubleJump = true;
    float turnSmoothVelocity;
    Transform cameraT;
    Rigidbody rb;
    Vector2 input;
    bool isJumping = false;
    public bool isOnGround;
    Animator anim;

    float jumpdelay=0f;

    void Awake()
    {
        //animator = GetComponent<Animator>();
        cameraT = Camera.main.transform;
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // input
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        input = input.normalized;

        if (Input.GetKeyDown(KeyCode.Space) && jumpdelay>0.1f &&(isOnGround||doubleJump))
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
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (input != Vector2.zero)
        {
            float targetRotation = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
            transform.Translate(Vector3.forward*moveSpeed);
        }
        if (isJumping && isOnGround)
        {
            Debug.Log("jump");
            rb.AddForce(Vector3.up * jumpPower);
            isJumping = false;
            jumpdelay = 0;
        }
        else if(isJumping && doubleJump)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpPower);
            doubleJump = false;
            isJumping = false;
            jumpdelay = 0;
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Lava"))
        {
            print("Padol si do lavy.");
            Destroy(gameObject);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Trap"))
        {
            print("Zabila ta trapka.");
            //Destroy(gameObject);
        } else if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            print("Zabil ta enemy.");
            DissolveEffect.dissolve = true;
        }
    }

}
