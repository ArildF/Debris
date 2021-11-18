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
        private AudioSource _source;
        private ThrustInfo _info;
        private bool _fading;


        private void Start()
        {
            _source = gameObject.GetComponent<AudioSource>();
        }

        [Inject]
        public void Init(ThrustInfo info)
        {
            _info = info;
        }

        void Update()
        {
            if (Abs(_info.CurrentDirectionalThrust) > 1)
            {
                Debug.Log($"current directional thrust: {_info.CurrentDirectionalThrust}");
                if (!_source.isPlaying)
                {
                    _source.clip = mainThrustersBurn;
                    _source.spread = 180;
                    _source.Play();
                }
                _source.volume = _info.CurrentDirectionalThrust / maxThrust;
            }
            else if (Abs(_info.CurrentRotationalThrust) > 1)
            {
                Debug.Log($"current rotational thrust: {_info.CurrentRotationalThrust}");
                float CalculateVolume() => (_info.CurrentRotationalThrust / maxThrust) * rotationalThrustFactor;
                if (!_source.isPlaying)
                {
                    _source.clip = mainThrustersBurn;
                    _source.spread = 180;
                    _source.volume = CalculateVolume();
                    _source.Play();
                    Debug.Log($"Started playing rotational thrust, volume is {_source.volume}, rotational thrust is {_info.CurrentRotationalThrust}");
                }
                else
                {
                    _source.volume = CalculateVolume();
                    Debug.Log(
                        $"Adjusted volume for rotational thrust, volume is {_source.volume}, rotational thrust is {_info.CurrentRotationalThrust}");
                }
            }
            else if (_source.isPlaying && !_fading && 
                Max(_info.CurrentDirectionalThrust, _info.CurrentRotationalThrust) < 1)
            {
                Debug.Log("Max " + Max(_info.CurrentDirectionalThrust, _info.CurrentRotationalThrust));
                StartCoroutine(FadeOut());
            }
        }

        private IEnumerator FadeOut()
        {
            try
            {
                Debug.Log("Started fading");
                _fading = true;
                var enumerator = _source.FadeOut(0.5f);
                while (enumerator.MoveNext())
                {
                    yield return null;
                }
            }
            finally
            {
                Debug.Log("Stopped fading");
                _fading = false;
            }
        }
    }
}