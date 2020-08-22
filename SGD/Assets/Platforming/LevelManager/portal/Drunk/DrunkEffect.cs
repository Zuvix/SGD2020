using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class DrunkEffect : MonoBehaviour
{
    private float noiseScale = 10.0f;
    private float refractionPower = 0.05f;
    private float twirl = 0.1f;
    private float speed = 0.1f;
    private float colorIntensity = -0.59f;
    public float speedTwirlChange = 5.0f;
    public float speedColorIntensityChange = 0.0045f;

    public Material material;
    public GameObject Plane;

    private void Awake()
    {
        material = GetComponent<Renderer>().material;
        material.SetFloat("_Twirl", twirl);
        material.SetFloat("_ColorIntensity", colorIntensity);
        StartDrunkEffect();
    }

    public IEnumerator Effect()
    {
        while (twirl < 600 || colorIntensity > -6.0f)
        {
            if (twirl < 600f) 
                ChangeTwirl(speedTwirlChange);
            if (colorIntensity > -6.0f && colorIntensity >= -1.5f)
                ChangeColorIntensity(speedColorIntensityChange);
            else if (colorIntensity < -1.5f && colorIntensity > -6.0f)
                ChangeColorIntensity(speedColorIntensityChange + 0.5f);
            yield return new WaitForFixedUpdate();
        }
        Plane.SetActive(false);
    }

    private void ChangeTwirl(float value)
    {
        this.twirl += value;
        material.SetFloat("_Twirl", twirl);
    }
    private void ChangeColorIntensity(float value)
    {
        this.colorIntensity -= value;
        material.SetFloat("_ColorIntensity", colorIntensity);
        
    }
    public void StartDrunkEffect()
    {
        Plane.SetActive(true);
        StartCoroutine("Effect");
    }
}
