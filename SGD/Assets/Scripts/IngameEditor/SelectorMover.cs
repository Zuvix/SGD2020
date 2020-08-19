using Management;
using UnityEngine;
using Utils;

namespace IngameEditor
{
    public class SelectorMover : MonoBehaviour
    {
        public Camera target;
        public Transform selector;
        public Transform rig;

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
            transform.position = Vector3.Scale(Scale() / 0.2f,  new Vector3(1, 0, -1));
            rig.position = Vector3.Scale(Scale() / 0.2f,  new Vector3(1, 0, -1));
        
            if (GameManager.instance.selectedGameObject == null) 
                return;
        
            // Raycast begin
            var ray = target.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, 20, 1 << 17))
            {
                selector.gameObject.SetActive(true);
                _hit = hit.point;
                selector.position = _hit.ToLevelCords();
            }
            else
            {
                _hit = null;
                selector.gameObject.SetActive(false);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            for (var x = -gridDimensions.x / 2; x <= gridDimensions.x / 2; x++)
            {
                for (var z = -gridDimensions.y / 2; z <= gridDimensions.y / 2; z++)
                {
                    Gizmos.DrawSphere(Scale() / 0.2f + new Vector3(x, 0f, -z).ToLevelCords(), 0.03f);
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
}