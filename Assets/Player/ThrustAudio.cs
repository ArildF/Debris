using System.Collections;
using UniDi;
using UnityEngine;
using Utilities;
using static UnityEngine.Mathf;

namespace Player
{
    public class ThrustAudio : MonoBehaviour
    {
        public AudioClip mainThrustersBurn;
        public float maxThrust = 600_000;
        public float rotationalThrustFactor = 0.3f;
        public AudioSource source;
        private ThrustInfo _info;
        private bool _fading;


        [Inject]
        public void Init(ThrustInfo info)
        {
            _info = info;
        }

        void Update()
        {
            if (Abs(_info.CurrentDirectionalThrust) > 1)
            {
                // Debug.Log($"current directional thrust: {_info.CurrentDirectionalThrust}");
                if (!source.isPlaying)
                {
                    source.clip = mainThrustersBurn;
                    source.spread = 180;
                    source.Play();
                }
                source.volume = _info.CurrentDirectionalThrust / maxThrust;
                _fading = false;
            }
            else if (Abs(_info.CurrentRotationalThrust) > 1)
            {
                // Debug.Log($"current rotational thrust: {_info.CurrentRotationalThrust}");
                float CalculateVolume() => (_info.CurrentRotationalThrust / maxThrust) * rotationalThrustFactor;
                if (!source.isPlaying)
                {
                    source.clip = mainThrustersBurn;
                    source.spread = 180;
                    source.volume = CalculateVolume();
                    source.Play();
                    // Debug.Log($"Started playing rotational thrust, volume is {source.volume}, rotational thrust is {_info.CurrentRotationalThrust}");
                }
                else
                {
                    source.volume = CalculateVolume();
                    // Debug.Log(
                    //     $"Adjusted volume for rotational thrust, volume is {source.volume}, rotational thrust is {_info.CurrentRotationalThrust}");
                }

                _fading = false;
            }
            else if (source.isPlaying && !_fading && 
                Max(_info.CurrentDirectionalThrust, _info.CurrentRotationalThrust) < 1)
            {
                // Debug.Log("Max " + Max(_info.CurrentDirectionalThrust, _info.CurrentRotationalThrust));
                StartCoroutine(FadeOut());
            }
        }

        private IEnumerator FadeOut()
        {
            try
            {
                _fading = true;
                var enumerator = source.FadeOut(0.5f);
                while (_fading && enumerator.MoveNext())
                {
                    yield return null;
                }
            }
            finally
            {
                _fading = false;
            }
        }
    }
}