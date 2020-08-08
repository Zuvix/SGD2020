using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingBlock : MonoBehaviour
{
    Rigidbody rb;
    private float maxLower = 0.02f;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    IEnumerator StartFalling()
    {
        yield return new WaitForSeconds(2f);
        rb.isKinematic = false;
    }
    IEnumerator MoveLower()
    {
        float lower=0;
        while (lower <= maxLower && rb.isKinematic)
        {
            transform.Translate(Vector3.down * 0.0015f);
            yield return new WaitForFixedUpdate();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy")|| collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine("MoveLower");
            StartCoroutine(StartFalling());
        }
    }
}
