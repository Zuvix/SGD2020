using UnityEngine;
using System.Collections;

public class CameraMover : MonoBehaviour
{

    [SerializeField]
    private Transform target;

    [SerializeField]
    private Vector3 offsetPosition = new Vector3(0f, 2.75f, -7.17f);

    [SerializeField]
    private float smoothSpeed = 13.5f;

    private void LateUpdate()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (target == null)
        {
            print("Player destroyed.");
            return;
        }
        target.transform.localPosition = offsetPosition;
        transform.position = Vector3.Lerp(transform.position, target.transform.position, Time.smoothDeltaTime * smoothSpeed);
        transform.rotation = target.rotation;
    }
}