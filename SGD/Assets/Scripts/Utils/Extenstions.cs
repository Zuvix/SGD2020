using UnityEngine;
namespace Utils
{
    public static class Extensions {
        public static Vector3 ToLevelCords(this Vector3 vector)
        {
            vector = new Vector3(Mathf.Ceil(vector.x) + (vector.x < 0 ? 0.5f : -0.5f), 0, Mathf.Ceil(vector.z) + (vector.z < 0 ? 0.5f : -0.5f));
            return vector;
        }
    }
}