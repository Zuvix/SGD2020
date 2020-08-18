using System;
using UnityEngine;

public class Icon : MonoBehaviour
{
    public Transform cam;
    private void Update()
    {
        if (cam != null)
        {
            transform.localRotation = Quaternion.Euler(cam.rotation.eulerAngles);
        }
    }
}
