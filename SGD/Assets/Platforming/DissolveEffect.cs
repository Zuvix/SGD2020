using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[RequireComponent(typeof(Renderer))]
public class DissolveEffect : MonoBehaviour
{
    public float dissolveValue = 1f;

    public Material material;
    private bool dissolve = false;
    public float dissolveSpeed=0.0075f;
    public GameObject master;

    private void Awake()
    {
        material = GetComponent<Renderer>().material;
        material.SetFloat("Dissolve", dissolveValue);
    }

    /*private void Update()
    {
        if (dissolve && dissolveValue < 1)
            setDissolve();

        if (dissolveValue >= 1)
            Destroy(transform.parent.gameObject);
    }*/
    public IEnumerator Dissolve()
    {
        while (dissolveValue < 1)
        {
            ChangeDissolve(dissolveSpeed);
            yield return new WaitForFixedUpdate();
            if(dissolveValue>=1)
                Destroy(master);
        }
        Destroy(master);
    }
    public IEnumerator Summon()
    {
        while (dissolveValue > 0)
        {
            ChangeDissolve(-dissolveSpeed*0.6f);
            yield return new WaitForFixedUpdate();
        }
    }

    private void ChangeDissolve(float value)
    {
        this.dissolveValue += value;
        if (dissolveValue < 0f)
            dissolveValue = 0f;
        if (dissolveValue > 1)
            dissolveValue = 1f;
        material.SetFloat("Dissolve", dissolveValue);
    }
    public void startDissolve()
    {
        StartCoroutine(Dissolve());
    }
}