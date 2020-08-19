using System;
using UnityEngine;

namespace Data
{
    [Serializable]
    public enum PlaceableType
    {
        Enemy,
        Trap,
        Gem
    }
    
    [Serializable]
    [CreateAssetMenu(fileName = "New Placeable Object", menuName = "Level/PlaceableObject")]
    public class PlaceableDataObject : ScriptableObject
    {
        public int id;
        public Mesh mesh;
        public Material material;
        public GameObject targetPrefab;
        
        [Header("ICON")]
        public Sprite icon;
        public Color iconColor;
    }
}