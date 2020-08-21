﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerBehaviour : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float jumpPower = 6f;
    public float turnSmoothTime = 0.1f;
    public bool doubleJump = true;
    public bool controlsEnabled = true;
    //public float accel = 50f;
    public float downForce = 10f;
    public float shadowDistanceFromGroud = 0.08f;
    public float leftGroundTime = 0f;
    public float attackCd = 2f;
    private float currentAttackCd = 20f;
    public AudioSource punch;
    public AudioSource walk1;
    public AudioSource walk2;
    public AudioSource jump;
    public AudioSource land;
    public AudioSource deathSound;

    public bool isAttacking = false;
    float turnSmoothVelocity;
    Transform cameraT;
    Rigidbody rb;
    Vector2 input;
    bool isJumping = false;
    public bool isOnGround;
     public bool hasJumpedOnce = false;
    Animator anim;
    private LayerMask mask;
    public float maxRayDistance = 1f;
    float jumpdelay = 0f;
    public GameObject shadow;
    public GameObject fistTrail;
    private bool isHit=false;

    void Awake()
    {
        mask = LayerMask.GetMask("Ground", "Enemy","LivingGround");
        cameraT = Camera.main.transform;
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // RAycasting for landing
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        Physics.Raycast(ray, out hit, maxRayDistance, mask);
        if (Physics.Raycast(ray, out hit, maxRayDistance, mask))
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
        Physics.Raycast(ray, out hit, maxRayDistance * 30, mask);
        shadow.transform.position = hit.point + Vector3.up * shadowDistanceFromGroud;

        // input
        if (controlsEnabled)
        {
            input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            input = input.normalized;
        }
        else
        {
            input = Vector2.zero;
        }
        //Input.GetKeyDown(KeyCode.Jump) --> Input.GetButton("Jump")
        if (Input.GetButtonDown("Jump") && jumpdelay > 0.1f && (leftGroundTime<0.15f || doubleJump)&&controlsEnabled)
        {
            isJumping = true;
        }
        jumpdelay += Time.deltaTime;
        if (input != Vector2.zero)
        {
            anim.SetBool("isRunning", true);
        }
        else
        {
            anim.SetBool("isRunning", false);
        }


        //TIMERS
        if (isOnGround)
        {
            leftGroundTime = 0f;
        }
        else
        {
            leftGroundTime += Time.deltaTime;
        }

        //Attacking
        if (isAttacking)
        {
            currentAttackCd = 0;
        }
        else
        {
            currentAttackCd += Time.deltaTime;
        }
        if (Input.GetButton("Attack") && currentAttackCd >= attackCd && controlsEnabled)
        {
            punch.Play();
            isAttacking = true;
            anim.SetBool("isAttacking", true);
            fistTrail.SetActive(true);
        }
    }
    private void LateUpdate()
    {
        isHit = false;
    }
    public void StopAttack()
    {
        isAttacking = false;
        anim.SetBool("isAttacking", false);
        fistTrail.SetActive(false);
    }
    public void Die()
    {
        if(!deathSound.isPlaying)
            deathSound.Play();
        controlsEnabled = false;
        gameObject.GetComponentInChildren<DissolveEffect>().StartDissolve();
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
           // cameraT.parent.rotation = transform.rotation;
        }
        if (isJumping && !hasJumpedOnce)
        {
            hasJumpedOnce = true;
            rb.AddForce(Vector3.up * jumpPower);
            isJumping = false;
            jumpdelay = 0;
            anim.SetBool("isJumping", true);
            anim.SetBool("isFalling", false);
            jump.Play();
            doubleJump = true;
        }
        else if (isJumping && doubleJump)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpPower);
            doubleJump = false;
            isJumping = false;
            jumpdelay = 0;
            anim.SetBool("isDjumping", true);
            anim.SetBool("isLanding", false);
            anim.SetBool("isFalling", false);
            anim.Play("jump", 0, 0);
            jump.Play();
        }
        if (rb.velocity.y < -0.15f && leftGroundTime>0.15f)
        {
            
            rb.AddForce(Vector3.down * downForce, ForceMode.Force);
            anim.SetBool("isFalling", true);
            anim.SetBool("isJumping", false);
            anim.SetBool("isDjumping", false);
        }

    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Void") && !isHit)
        {
            Die();
            isHit = true;
        }
        if (other.gameObject.CompareTag("Enemy") && !isHit)
        {
            if (other.gameObject.GetComponent<Enemy>().summoningComplete)
            {
                if (isAttacking)
                {
                    Enemy e = other.gameObject.GetComponent<Enemy>();
                    e.Die();
                    e.Impact();
                }
                else if (rb.velocity.y < -0.05f)
                {
                    other.gameObject.GetComponent<Enemy>().Die();
                    rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                    rb.AddForce(Vector3.up * jumpPower);
                    anim.SetBool("isJumping", true);
                    anim.SetBool("isLanding", false);
                    doubleJump = true;
                    jump.Play();
                }
                else
                {
                    Die();
                    isHit = true;
                }
            }
        }
        if (other.gameObject.CompareTag("Spike")|| other.gameObject.CompareTag("Projectile") && !isHit)
        {
            Die();
            isHit = true;
        }
        if (other.gameObject.CompareTag("Portal")&&!isHit)
        {
            LevelManager.Instance.FinishLevel();
            isHit = true;
        }
        /*if (other.gameObject.CompareTag("Iceball"))
        {
            if (!isAttacking)
            {
                Die();
                isHit = true;
            }
            other.gameObject.GetComponent<Projectile>().Pop();
        }*/
    }
    public void Walk1()
    {
        walk1.Play();
    }
    public void Walk2()
    {
        walk2.Play();
    }
    public void Land()
    {
        land.Play();
    }

}
