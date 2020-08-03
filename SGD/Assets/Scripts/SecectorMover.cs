using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class SecectorMover : MonoBehaviour
{
    public Camera target;
    public GameObject selector;
    public GameObject prefab;

    public Vector2 gridDimensions;
    private Vector3? _hit;
    private BoxCollider _collider;

    private Vector3 Scale() => new Vector3(gridDimensions.x / 10, 0, gridDimensions.y / 10);

    private void Start()
    {
        _collider = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        transform.localScale = Scale();
        // Raycast begin
        var ray = target.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit))
        {
            selector.SetActive(true);
            _hit = hit.point;
            selector.transform.position = _hit.ToLevelCords();
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Place block");
            }
        }
        else
        {
            _hit = null;
            selector.SetActive(false);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        for (var x = -gridDimensions.x / 2; x <= gridDimensions.x / 2; x++)
        {
            for (var z = -gridDimensions.y / 2; z <= gridDimensions.y / 2; z++)
            {
                Gizmos.DrawSphere(new Vector3(x, 0f, z).ToLevelCords(), 0.03f);
            }

        }

        if (_hit.HasValue)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(target.transform.position, _hit.GetValueOrDefault());
            Gizmos.DrawSphere(_hit.GetValueOrDefault(), 0.03f);
        }
    }
}