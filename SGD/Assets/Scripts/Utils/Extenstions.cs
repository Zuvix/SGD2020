using Data;
using UnityEngine;
namespace Utils
{
    public static class Extensions {
        public static Vector3 ToLevelCords(this Vector3? vector)
        {
            if (vector.HasValue)
            {
                vector = new Vector3((vector.GetValueOrDefault().x < 0 ? Mathf.Floor(vector.GetValueOrDefault().x) : Mathf.Ceil(vector.GetValueOrDefault().x)) + (vector.GetValueOrDefault().x < 0 ? 0.5f : -0.5f), 0,
                                     (vector.GetValueOrDefault().z < 0 ? Mathf.Floor(vector.GetValueOrDefault().z) : Mathf.Ceil(vector.GetValueOrDefault().z)) + (vector.GetValueOrDefault().z < 0 ? 0.5f : -0.5f));
                return vector.GetValueOrDefault();
            }
            else
            {
                return Vector3.zero;
            }
        }
        
        public static Vector3 ToLevelCords(this Vector3 vector)
        {
            vector = new Vector3((vector.x < 0 ? Mathf.Floor(vector.x) : Mathf.Ceil(vector.x)) + (vector.x < 0 ? 0.5f : -0.5f), 0,
                                 (vector.z < 0 ? Mathf.Floor(vector.z) : Mathf.Ceil(vector.z)) + (vector.z < 0 ? 0.5f : -0.5f));
                return vector;
        }
    }
}