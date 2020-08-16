using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed = 0.1f;
    public IEnumerator Fly()
    {
        yield return new WaitForSeconds(0.95f);
        while (true)
        {
            transform.Translate(Vector3.up * speed*-1);
            transform.Rotate(Vector3.up * speed*10);
            yield return new WaitForFixedUpdate();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player")|| other.gameObject.CompareTag("Enemy")|| other.gameObject.CompareTag("Wall"))
        {
            gameObject.SetActive(false);
        }
    }
    private void OnEnable()
    {
        StartCoroutine("Fly");
    }
    private void OnDisable()
    {
        StopCoroutine("Fly");
    }

}
