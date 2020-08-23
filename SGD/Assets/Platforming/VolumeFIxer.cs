using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeFIxer : MonoBehaviour
{
    public float setVolume = 0.6f;
    private void Awake()
    {
        AudioListener.volume = setVolume;
    }
}
