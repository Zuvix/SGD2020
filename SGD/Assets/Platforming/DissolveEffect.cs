using UnityEngine;
[RequireComponent(typeof(Renderer))]
public class DissolveEffect : MonoBehaviour
{
    //[//] private float noiseStrength = 0.25f;
    [SerializeField] public static float dissolveValue = 0.00f;

    public Material material;
    private static bool dissolve = false;
    public void StartDissolve()
    {
        dissolve = true;
    }
    private void Start()
    {
        material = GetComponent<Renderer>().material;    
    }

    private void Update()
    {
        if (dissolve && dissolveValue < 1)
            SetDissolve();

        if (dissolveValue >= 1)
            Destroy(gameObject);
    }

    private void SetDissolve()
    {
        dissolveValue += 0.001f;
        material.SetFloat("Dissolve", dissolveValue);
    }
}