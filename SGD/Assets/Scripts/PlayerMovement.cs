using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Transform cam;
    public CharacterController controller;

    public float speed = 4f;
    public float gravity = 6f;

    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    public float jumpHeigh = 3f;
    private bool doubleJump = true;
    Vector3 velocity;
    bool isGrounded;
    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;


    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float hor = Input.GetAxisRaw("Horizontal");
        float ver = Input.GetAxisRaw("Vertical");
        Vector3 dir = new Vector3(hor * 0.3f, 0f, ver).normalized;
        transform.Translate(dir, Space.Self);

        if (dir.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                     controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }
        if (isGrounded)
            doubleJump = true;
        
        if (Input.GetButtonDown("Jump") )
        {
            if (isGrounded)
            {
              //  print(Input.GetAxisRaw("Vertical") + " --- " + Input.GetAxisRaw("Horizontal"));
             //   if (Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0) { 
              //      print("stlacena sipka");
              //      velocity.y = jumpHeigh * 0.5f;
                //} else { 
                     velocity.y = jumpHeigh;
           // }
        }
            else if (doubleJump)
            {
                velocity.y = jumpHeigh;
                doubleJump = false;
                //print("2. jump");
            }
        }
  
        velocity.y -= gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
