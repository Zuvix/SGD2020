using System;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(menuName = "LDO")]
    [Serializable]
    public class LevelDataObject : ScriptableObject
    {
        public List<LevelData> levels;
    }
}