using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TrapBehaviour : MonoBehaviour
{
    public float activeTime=2.5f;
    public float passiveTime=5f;
    public float offset=0f;
    Animator anim;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    private void Start()
    {
        StartCoroutine("TrapLogic");
    }
    IEnumerator TrapLogic()
    {
        offset = offset % Convert.ToInt32(Math.Floor(activeTime + passiveTime));
        if (passiveTime > offset)
        {
            yield return new WaitForSeconds(passiveTime - offset);
            anim.SetBool("isActive", true);
            yield return new WaitForSeconds(activeTime);
        }
        else
        {
            anim.SetBool("isActive", true);
            yield return new WaitForSeconds(passiveTime+activeTime - offset);
        }
        while (true)
        {
            anim.SetBool("isActive", false);
            yield return new WaitForSeconds(passiveTime);
            anim.SetBool("isActive", true);
            yield return new WaitForSeconds(activeTime);
        }

    }
}
