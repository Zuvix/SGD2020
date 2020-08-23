using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectGems : MonoBehaviour
{
    public GameObject particleBoom;
    public GameObject spirits;
    Light l;
    private bool hit;
    Renderer m;
    Collider c;
    [HideInInspector] public Vector3 baseScale;
    public float upscaleSpeed = 0.002f;
    private void Awake()
    {
        m = GetComponent<Renderer>();
        c = GetComponent<Collider>();
        l = GetComponent<Light>();
        baseScale = transform.localScale;
        transform.localScale = Vector3.zero;
        StartCoroutine(WaitAndAppear());
    }

    // Update is called once per frame
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !hit && enabled)
        {
            l.enabled = false;
            spirits.SetActive(false);
            c.enabled = false;
            m.enabled = false;
            particleBoom.SetActive(true);
            hit = true;
            Destroy(this.gameObject, 1f);
            LevelManager.Instance.GetGem();
        }
    }
    public IEnumerator WaitAndAppear()
    {
        c.enabled = false;
        l.enabled = false;
        while (transform.localScale.x<baseScale.x || transform.localScale.y < baseScale.y || transform.localScale.z < baseScale.z)
        {
            Vector3 modif = new Vector3(0,0,0);
            if (transform.localScale.x < baseScale.x)
            {
                modif.x += upscaleSpeed;
            }
            if (transform.localScale.y < baseScale.y)
            {
                modif.y += upscaleSpeed;
            }
            if (transform.localScale.z < baseScale.z)
            {
                modif.z += upscaleSpeed;
            }
            transform.localScale += modif;
            yield return new WaitForFixedUpdate();
        }
        //yield return new WaitForSeconds(0.65f);
        c.enabled = true;
        l.enabled = true;
    }
}
