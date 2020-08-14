using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Renderer))]
public class FrogDissolveEffect : MonoBehaviour
{
    public float dissolveValue = 0.00f;

    public Material material;
    private bool dissolve = false;
    public GameObject gm; 

    private void Start()
    {        
        material = GetComponent<Renderer>().material;
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
        this.dissolveValue += 0.0075f;
        material.SetFloat("Dissolve", dissolveValue);
    }
    public void startDissolve()
    {
        Debug.Log("Disovling frog");
        if(gm.activeSelf)
            gm.GetComponent<CoronaFrogDissolveEffect>().startDissolve();
        dissolve = true;
    }
}