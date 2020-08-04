using System;
using UnityEngine;

namespace Data
{
    [Serializable]
    [CreateAssetMenu(fileName = "New Block",menuName = "Level/Block")]
    public class BlockDataObject : ScriptableObject
    {
        public int id;
        public Mesh mesh;
        public Material material;
        public GameObject targetPrefab;
        public PlaceableDataObject[] defaultPlacings = new PlaceableDataObject[9];
    }
}