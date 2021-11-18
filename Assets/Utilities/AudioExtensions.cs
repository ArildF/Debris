using System.Collections;
using UnityEngine;

namespace Utilities
{
    public static class AudioExtensions
    {
        public static IEnumerator FadeOut(this AudioSource self, float time)
        {
            var volume = self.volume;
            float startVolume = volume;
            float step = volume / time;
            Debug.Log($"Starting fade, step {step}");
            while (self.volume > Mathf.Epsilon)
            {
                Debug.Log($"Fading at {self.volume}");
                self.volume -= step * Time.deltaTime;
                yield return null;
            }
            self.Stop();
            self.volume = startVolume;
        }
    }
}