using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalOpener : MonoBehaviour
{
    public GameObject Portal;
    private void Awake()
    {
        Portal.SetActive(false);
    }
    public void OpenPortal()
    {
        Portal.SetActive(true);
        
    }
}
