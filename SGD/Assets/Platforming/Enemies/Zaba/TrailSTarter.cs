using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailSTarter : MonoBehaviour
{
    public TrailRenderer tr;
    public DissolveEffect disolver;
    // Start is called before the first frame update
    private void Awake()
    {
        disolver = GetComponent<DissolveEffect>();
    }
    private void OnEnable()
    {
        StartCoroutine(W8andGo());
    }
    public void ActivateTr()
    {
        tr.enabled = true;

    }
   IEnumerator W8andGo()
    {
        yield return new WaitForFixedUpdate();
        disolver.StartSummon();
        Invoke("ActivateTr", 0.75f);
    }
}
