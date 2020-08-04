using UnityEngine;
using UnityEngine.Rendering;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public Animator animator;

    public float speed = 7;
    public float _rotationSpeed = 90;
    private Vector3 rotation;

    public float gravity = -8f;
    public float jumpHeigh = 3f;
    private bool doubleJump = true;
    Vector3 move;
    bool isGrounded;
    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;
    public void Update()
    {
        Vector3 jump = new Vector3(0.0f, 2.0f, 0.0f);
        move = new Vector3(0, 0, Input.GetAxisRaw("Vertical") * Time.deltaTime);

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && move.y < 0)
        {
            move.y = -2f;
        }
        if (isGrounded)
        {
            doubleJump = true;
            animator.SetInteger("jump", 0);
            animator.SetInteger("condition", 0);
        }
        if ((isGrounded && (Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0)))
        {
            doubleJump = true;
            animator.SetInteger("jump", 0);
            animator.SetInteger("condition", 1);
        }
        else if ((isGrounded && (Input.GetAxisRaw("Vertical") == 0 && Input.GetAxisRaw("Horizontal") == 0)))
        {
            animator.SetInteger("jump", 0);
            animator.SetInteger("condition", 0);
        }
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                //  print(Input.GetAxisRaw("Vertical") + " --- " + Input.GetAxisRaw("Horizontal"));
                //   if (Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0) { 
                //      print("stlacena sipka");
                //      velocity.y = jumpHeigh * 0.5f;
                //} else { 
                move.y = jumpHeigh;
                animator.SetInteger("jump", 1);
                animator.SetInteger("condition", 3);
                print("jump");
                // }

            }
            else if (doubleJump)
            {
                move.y = jumpHeigh;
                doubleJump = false;
                //print("2. jump");
            }
        }

        this.rotation = new Vector3(0, Input.GetAxisRaw("Horizontal") * _rotationSpeed * Time.deltaTime, 0);

        move = this.transform.TransformDirection(move);
        move.y += gravity * Time.deltaTime;

        controller.Move(move * speed);
       // controller.Move(move * _speed);
        this.transform.Rotate(this.rotation);

    }
}