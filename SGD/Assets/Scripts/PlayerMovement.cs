using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 6.0F;
    public float jumpSpeed = 8.0F;
    public float gravity = 6F;
    public float rotateSpeed = 1.0F;
    private float verticalSpeed = 0;
    public CharacterController characterController;

    private int currentJump = 0;
    private const int MAX_JUMP = 2;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }
    void Move()
    {
        float horizontalMove = Input.GetAxis("Horizontal");
        float verticalMove = Input.GetAxis("Vertical");

        if (characterController.isGrounded)
            verticalSpeed = 0;
        else
            verticalSpeed -= gravity * Time.deltaTime;

        Vector3 gravityMove = new Vector3(0, verticalSpeed, 0);
        Vector3 move = transform.forward * verticalMove + transform.right * horizontalMove;

        //jump todo, doublejump todo
        if (characterController.isGrounded && Input.GetButton("Jump"))
        {
            print(currentJump + " prva");
            currentJump = 0;
            move.y = jumpSpeed;
            //move.y += gravity * Time.deltaTime;
            currentJump++;
        }
        else if (!characterController.isGrounded && currentJump < MAX_JUMP && Input.GetButton("Jump"))
        {
            print(currentJump + " druhaaaaaa");
            move.y = jumpSpeed;
            currentJump++;
        }

        /*        if(0 < horizontalMove) { 
                    //horizontalMove -= 2f;
                    transform.Rotate(0, horizontalMove, 0);
                }
                else if (0 > horizontalMove)
                {
                    //horizontalMove += 2f;
                    transform.Rotate(0, horizontalMove, 0);
                }*/

        //move.y += gravity * Time.deltaTime;
        transform.Rotate(0, horizontalMove, 0);
        characterController.Move(speed * Time.deltaTime * move + gravityMove * Time.deltaTime);
        transform.Rotate(0, horizontalMove, 0);
    }
}











