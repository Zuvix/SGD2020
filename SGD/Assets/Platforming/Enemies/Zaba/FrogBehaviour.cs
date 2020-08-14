using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogBehaviour : Enemy
{
    public Vector3 targetPosition;
    public float alpha = 60;
    public float force;
    public override void  Activate()
    {
        Jump();
    }
    public void Jump()
    {
        float x0 = transform.position.x;
        float y0 = transform.position.y;
        float d = Vector3.Distance(transform.position, new Vector3(targetPosition.x, transform.position.y, targetPosition.z));
        float v0 = Mathf.Sqrt(d * 9.81f / (Mathf.Sin(2 * alpha * Mathf.Deg2Rad)));
        float force = v0 * rb.mass;
        Vector3 movementVector = (transform.forward + (transform.up * 2)).normalized;
        rb.AddForce(movementVector * force, ForceMode.Impulse);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    private void Start()
    {
        Activate();
    }
}
