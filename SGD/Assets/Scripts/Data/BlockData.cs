using System;
using UnityEngine;

namespace Data
{
    [Serializable]
    [CreateAssetMenu(menuName = "Block")]
    public class BlockData : ScriptableObject
    {
        public int id;
        public Mesh mesh;
        public Material material;
        public GameObject[] defaultPlacings = new GameObject[9];
    }
}