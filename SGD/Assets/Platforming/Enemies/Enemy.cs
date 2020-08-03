using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public bool customGravity = false;
    public bool isOnGround = false;
    public float gravityForce = 9.81f;
    protected Animator anim;
    protected Rigidbody rb;
    //Nejaka animacia ked zabija hraca
    public abstract void Kill();
    // Ked umiera enemak
    public abstract void Die();
    //Aktivovanie nepriatela, aby sa zacal spravat ako je definovane v GDD
    public abstract void Activate();

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
        }

    }
    public virtual void Awake()
    {
        if((rb= GetComponent<Rigidbody>())== null)
        {
            Debug.Log("Rigidbody not found");
        }
        anim = GetComponent<Animator>();
        if ((anim = GetComponent<Animator>()) == null)
        {
            Debug.Log("Animator not found");
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        isOnGround = false;
    }
    private void FixedUpdate()
    {
        if (customGravity && isOnGround)
        {
            rb.AddForce(Vector3.down * gravityForce);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.CompareTag("Spike"))
        {
            this.gameObject.GetComponent<Enemy>().Die();
        }
    }
}
