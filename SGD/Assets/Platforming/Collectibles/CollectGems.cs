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
    private void Awake()
    {
        m = GetComponent<Renderer>();
        c = GetComponent<Collider>();
        l = GetComponent<Light>();
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
        yield return new WaitForSeconds(0.65f);
        c.enabled = true;
        l.enabled = true;
    }
}
