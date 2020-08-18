using System;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(menuName = "Level Data Object")]
    [Serializable]
    public class LevelDataObject : ScriptableObject
    {
        public List<LevelData> levels;
    }
}