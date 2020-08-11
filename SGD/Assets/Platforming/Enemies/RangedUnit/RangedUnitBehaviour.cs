using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class RangedUnitBehaviour : Enemy
{
    // Start is called before the first frame update
    private void Start()
    {
        Activate();
    }
    public override void Activate()
    {
        //StartCoroutine("BrainScope");
        Debug.Log("Ranged Unit Activated");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Kill()
    {
        throw new System.NotImplementedException();
    }

    public override void Die()
    {
        throw new System.NotImplementedException();
    }
    IEnumerator BrainScope()
    {
        StartCoroutine("AttacLogic");
        print("sem pridem");
        yield return new WaitForFixedUpdate();
    }
    IEnumerator AttacLogic()
    {
        while (true)
        {
        
        }
    }
}
