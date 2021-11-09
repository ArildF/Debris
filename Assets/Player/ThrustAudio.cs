using System;
using UniDi;
using UnityEngine;

namespace Player
{
    public class ThrustAudio : MonoBehaviour
    {
        public AudioClip mainThrustersBurn;
        public float maxThrust = 600_000;
        private AudioSource _source;
        private ThrustInfo _info;
        

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
            if (!_source.isPlaying && Mathf.Abs(_info.CurrentDirectionalThrust) > 1)
            {
                _source.clip = mainThrustersBurn;
                _source.volume = _info.CurrentDirectionalThrust / maxThrust;
                _source.spread = 180;
                _source.Play();
            }

            if (Mathf.Abs(_info.CurrentDirectionalThrust) < 1)
            {
                _source.Stop();
            }
        }
    }
}