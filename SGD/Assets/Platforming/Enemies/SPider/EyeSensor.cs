using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeSensor : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private float maxRayDistance = 2;
    [HideInInspector]
    public bool isOverGround=false;
    public bool isEnemyAhead = false;
    [HideInInspector]
    public float timeFromGround = 0f;
    public float timeOnGround = 0f;
    public float timeCloseToEnemy = 0f;
    public LayerMask mask;
    void Start()
    {
         mask = LayerMask.GetMask("Trap","Ground","Enemy","LivingGround");
    }

    void FixedUpdate()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxRayDistance,mask)) {
            if (hit.transform.gameObject.layer==8 || hit.transform.gameObject.layer == 14)
            {
                isOverGround = true;
                timeFromGround = 0f;
                timeOnGround += Time.deltaTime;
                Debug.DrawRay(transform.position, Vector3.down*maxRayDistance, Color.green);
            }
            else
            {
                isOverGround = false;
                timeFromGround += Time.deltaTime;
                timeOnGround = 0;
            }
            if (hit.transform.gameObject.layer==10|| hit.transform.gameObject.layer == 11)
            {
                isEnemyAhead= true;
                timeCloseToEnemy += Time.deltaTime;
                Debug.DrawRay(transform.position, Vector3.down*maxRayDistance, Color.black);
            }
            else{
                isEnemyAhead = false;
                timeCloseToEnemy = 0;
            }
        }
    else
        {
            isOverGround = false;
            isEnemyAhead = false;
            timeFromGround += Time.deltaTime;
            timeCloseToEnemy = 0;
            timeOnGround = 0;
            Debug.DrawRay(transform.position, Vector3.down* maxRayDistance, Color.red);
        }
    }
}
