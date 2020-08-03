using System;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    public Camera target;
    public float xSpeed = 100.0f;
    public float ySpeed = 100.0f;
    public float smoothTime = 20f;

    private float _rotationYAxis = 0.0f;
    private float _rotationXAxis = 0.0f;
    private float _velocityX = 0.0f;
    private float _velocityY = 0.0f;
    private float _velocityZoom = 0.0f;
    
    private void Start()
    {
        target = GetComponentInChildren<Camera>();
    }

    private void LateUpdate()
    {
        if (target)
        {
            target.transform.LookAt(transform, Vector3.up); 
            target.transform.localRotation = Quaternion.identity;
            // drag
            if (Input.GetMouseButton(1))
            {
                _velocityX += xSpeed * (Input.GetAxis("Mouse X") * 0.02f);
                _velocityY += ySpeed * (Input.GetAxis("Mouse Y") * 0.02f);
            }
            // zoom
            if (Math.Abs(Input.GetAxis("Mouse ScrollWheel")) > 0.01f)
            {
                _velocityZoom -= Input.GetAxis("Mouse ScrollWheel") * ((xSpeed + ySpeed) / 20);
            }
            _rotationYAxis += _velocityX;
            _rotationXAxis -= _velocityY;
            var fromRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
            var toRotation = Quaternion.Euler(_rotationXAxis, _rotationYAxis, 0);
            var rotation = toRotation;
            
            transform.rotation = rotation;
            target.transform.localPosition += Vector3.back * _velocityZoom;
            _velocityX = Mathf.Lerp(_velocityX, 0, Time.smoothDeltaTime * smoothTime);
            _velocityY = Mathf.Lerp(_velocityY, 0, Time.smoothDeltaTime * smoothTime);
            _velocityZoom = Mathf.Lerp(_velocityZoom, 0, Time.smoothDeltaTime * smoothTime);
        }
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, -target.transform.localPosition.z);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, target.transform.position);
    }
}