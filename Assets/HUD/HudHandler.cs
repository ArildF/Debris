using System;
using System.Globalization;
using TMPro;
using UniDi;
using UnityEngine;

namespace HUD
{
    public class HudHandler : MonoBehaviour
    {
        public TMP_Text text;
        private HudInfo _hudInfo;

        private void Update()
        {
            text.text = _hudInfo.AbsoluteVelocity.ToString(CultureInfo.InvariantCulture);
        }

        [Inject]
        public void Init(HudInfo hudInfo)
        {
            _hudInfo = hudInfo;
        }
    }
}