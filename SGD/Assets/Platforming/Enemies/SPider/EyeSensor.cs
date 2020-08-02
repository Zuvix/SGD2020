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
    public bool isOverTrap = false;
    [HideInInspector]
    public float timeFromGround = 0f;
    public float timeOnGround = 0f;
    public float timeOverTrap = 0f;
    public LayerMask mask;
    void Start()
    {
         mask = LayerMask.GetMask("Trap","Ground");
    }

    void FixedUpdate()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxRayDistance,mask)) {
            if (hit.transform.gameObject.layer==8)
            {
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
            }
            if (hit.transform.gameObject.layer==9)
            {
                isOverTrap = true;
                timeOverTrap +=Time.deltaTime;
                Debug.DrawRay(transform.position, Vector3.down, Color.black);
            }
            else{
                isOverTrap = false;
                timeOverTrap =0;
            }
        }
    else
        {
            isOverGround = false;
            isOverTrap = false;
            timeFromGround += Time.deltaTime;
            timeOverTrap =0;
            timeOnGround = 0;
            Debug.DrawRay(transform.position, Vector3.down, Color.red);
        }
    }
}
