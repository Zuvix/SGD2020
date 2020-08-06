using UnityEngine;
[RequireComponent(typeof(Renderer))]
public class DissolveEffect : MonoBehaviour
{
    //[//] private float noiseStrength = 0.25f;
    [SerializeField] public static float dissolveValue = 0.00f;

    public Material material;
    public static bool dissolve = false;

    private void Start()
    {
        material = GetComponent<Renderer>().material;    
    }

    private void Update()
    {
        if (dissolve && dissolveValue < 1)
            setDissolve();

        if (dissolveValue >= 1)
            Destroy(gameObject);
    }

    private void setDissolve()
    {
        dissolveValue += 0.001f;
        material.SetFloat("Dissolve", dissolveValue);
    }
}