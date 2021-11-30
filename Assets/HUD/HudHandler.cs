using System;
using System.Globalization;
using Player;
using TMPro;
using UniDi;
using UnityEngine;

namespace HUD
{
    public class HudHandler : MonoBehaviour
    {
        public TMP_Text textVelocity;
        public TMP_Text textViewDirection;
        private HudInfo _hudInfo;
        private PlayerViewInfo _playerViewInfo;
        private ViewDirection _lastViewDirection = ViewDirection.Front;

        void Start()
        {
            SetViewDirectionText();
        }

        private void Update()
        {
            textVelocity.text = _hudInfo.AbsoluteVelocity.ToString("F0", CultureInfo.InvariantCulture);
            if (_lastViewDirection != _playerViewInfo.ViewDirection)
            {
                SetViewDirectionText();
            }
        }

        private void SetViewDirectionText()
        {
            textViewDirection.text = _playerViewInfo.ViewDirection switch
            {
                ViewDirection.Front => "",
                var dir => $"{dir} view",
            };
            _lastViewDirection = _playerViewInfo.ViewDirection;
        }

        [Inject]
        public void Init(HudInfo hudInfo, PlayerViewInfo playerViewInfo)
        {
            _hudInfo = hudInfo;
            _playerViewInfo = playerViewInfo;
        }
    }
}