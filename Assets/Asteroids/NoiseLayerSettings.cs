using System;

namespace Asteroids
{
    [Serializable]
    public class NoiseLayerSettings
    {
        public enum FilterType { Simple, Ridged };
        public FilterType filterType;

        public bool enabled;
        [ConditionalHide("filterType", 0)]
        public SimpleNoiseSettings simpleNoiseSettings;
        [ConditionalHide("filterType", 1)]
        public RidgedNoiseSettings ridgedNoiseSettings;
    }
}