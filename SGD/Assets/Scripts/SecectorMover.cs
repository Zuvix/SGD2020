using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class SecectorMover : MonoBehaviour
{
    private Camera target;
    public GameObject selector;

    private void Start()
    {
        target = GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        var ray = target.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit))
        {
            selector.SetActive(true);
            selector.transform.position = hit.point.ToLevelCords();
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Place block");
            }
        }
        else
        {
            selector.SetActive(false);
        }
    }
}
