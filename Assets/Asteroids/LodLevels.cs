using System;
using UnityEngine;

namespace Asteroids
{
    public class LodLevels : ScriptableObject
    {
        public LodLevel[] levels = new[]
        {
            new LodLevel(0, 1, 0.5f),
            new LodLevel(1, 5, 0.1f),
            new LodLevel(2, 20, 0.01f),
            new LodLevel(3, 100, 0.001f),
        };
        
    }
    
    [Serializable]
    public class LodLevel
    {
        public int Level;
        public float DivideBy;
        public float ScreenSizeFade;

        public LodLevel(int level, float divideBy, float screenSizeFade)
        {
            Level = level;
            DivideBy = divideBy;
            this.ScreenSizeFade = screenSizeFade;
        }
    }


}