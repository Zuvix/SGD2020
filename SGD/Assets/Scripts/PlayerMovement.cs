using UnityEngine;
using UnityEngine.Rendering;

public class PlayerMovement : MonoBehaviour
{
    // public CharacterController controller;

    public Animator animator;

    public float speed = 5;
    public float _rotationSpeed = 90;
    private Vector3 rotation;

    public float gravity = -7f;
    public float jumpHeigh = 3f;
    private bool doubleJump = true;
    Vector3 move;
    bool isGrounded;
    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;

    public Rigidbody rb;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Update()
    {
        //Vector3 jump = new Vector3(0.0f, 2.0f, 0.0f);
        move = new Vector3(0, 0, Input.GetAxisRaw("Vertical") * Time.deltaTime);
        // isGrounded = Physics.CheckSphere(groundChecker.position, GroundDistance, Ground, QueryTriggerInteraction.Ignore);

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask, QueryTriggerInteraction.Ignore);

        if (isGrounded)
        {
            print("grouended");
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

                rb.AddForce(new Vector3(0, 5, 0), ForceMode.Impulse);
                //move.y = jumpHeigh;
                animator.SetInteger("jump", 1);
                animator.SetInteger("condition", 3);
            }
            else if (doubleJump)
            {
                rb.AddForce(new Vector3(0, 5, 0), ForceMode.Impulse);
                //move.y = jumpHeigh;
                doubleJump = false;
            }
        }

        this.rotation = new Vector3(0, Input.GetAxisRaw("Horizontal") * _rotationSpeed * Time.deltaTime, 0);
        transform.Translate(move * speed);
        this.transform.Rotate(this.rotation);

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Lava"))
        {
            print("Padol si do lavy.");
            Destroy(gameObject);
        } else if (other.gameObject.layer == LayerMask.NameToLayer("Trap"))
        {
            print("Zabila ta trapka.");
            //Destroy(gameObject);
        }
    }

}