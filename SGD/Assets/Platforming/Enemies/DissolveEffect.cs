using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[RequireComponent(typeof(Renderer))]
public class DissolveEffect : MonoBehaviour
{
    public float dissolveValue = 0.00f;

    public Material material;
    private bool dissolve = false;
    public float dissolveSpeed=0.0075f;

    private void Start()
    {        
        material = GetComponent<Renderer>().material;
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
        while (dissolveValue <= 1f)
        {
            ChangeDissolve(dissolveSpeed);
            yield return new WaitForFixedUpdate();
        }
        Destroy(transform.parent.gameObject);
    }
    public IEnumerator Summon()
    {
        while (dissolveValue >= 0f)
        {
            ChangeDissolve(-dissolveSpeed);
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