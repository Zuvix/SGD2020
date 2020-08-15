using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingBlock : MonoBehaviour
{
    Rigidbody rb;
    Material m;
    private float maxLower = 0.02f;
    bool activated = false;
    public AudioSource fallingSound;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        m = GetComponent<Renderer>().material;
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
            transform.Translate(Vector3.down * 0.0018f);
            yield return new WaitForFixedUpdate();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy")|| collision.gameObject.CompareTag("Player") &&activated==false)
        {
            activated = true;
            StartCoroutine("MoveLower");
            SetEmmision(false);
            fallingSound.Play();
            StartCoroutine(StartFalling());
            Invoke("Destruction", 12f);
        }
    }
    public void SetEmmision(bool toEmit)
    {
        if (toEmit)
            m.SetColor("_EmissionColor", Color.white);
        else
            m.SetColor("_EmissionColor", Color.black);
    }
    void Destruction()
    {
        Destroy(this.gameObject);
    }
}
