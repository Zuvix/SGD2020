using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalOpener : MonoBehaviour
{
    public GameObject Portal;
    public AudioSource shimmmer;
    private void Awake()
    {
        Portal.SetActive(false);
    }
    public void OpenPortal()
    {
        Portal.SetActive(true);
        StartCoroutine(StartShimmer());
    }
    IEnumerator StartShimmer()
    {
        while (shimmmer.volume < 0.9f)
        {
            shimmmer.volume += 0.01f;
            yield return new WaitForSeconds(0.05f);
        }
    }
}
