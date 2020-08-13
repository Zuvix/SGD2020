using UnityEngine;
[RequireComponent(typeof(Renderer))]
public class DissolveEffect : MonoBehaviour
{
    public float dissolveValue = 0.00f;

    public Material material;
    private bool dissolve = false;
    public GameObject gm; 

    private void Start()
    {        
        material = GetComponent<Renderer>().material;
        gm = GetComponent<GameObject>();
    }

    private void Update()
    {
        if (dissolve && dissolveValue < 1)
            setDissolve();

        if (dissolveValue >= 1)
            Destroy(transform.parent.gameObject);
    }

    private void setDissolve()
    {
        this.dissolveValue += 0.008f;
        material.SetFloat("Dissolve", dissolveValue);
    }
    public void startDissolve()
    {
        dissolve = true;
    }
}