using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeSensor : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private float maxRayDistance = 10;
    [HideInInspector]
    public bool isOverGround=false;
    [HideInInspector]
    public float timeFromGround = 0f;
    public float timeOnGround = 0f;
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxRayDistance)) {
            isOverGround = true;
            timeFromGround = 0f;
            timeOnGround += Time.deltaTime;
            Debug.DrawRay(transform.position, Vector3.down, Color.green);
        }
        else
        {
            isOverGround = false;
            timeFromGround += Time.deltaTime;
            timeOnGround = 0;
            Debug.DrawRay(transform.position, Vector3.down, Color.red);
        }
    }
}
