using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Triplano.Assets.SoundBox5
{
    [CreateAssetMenu(fileName = "New Oneshot Sound", menuName = "Audio/SoundTrack")]
    public class SB_Config_Soundtrack : SB_Config_Base
    {
        [Header("Fade In Settings")]
        public bool applyFadeIn;
        public SoundFadeSettings fadeInSettings;
        [Header("Fade Out Settings")]
        public bool applyFadeOut;
        public SoundFadeSettings fadeOutSettings;

        private SoundBox _attachedSoundBox;


        private void OnDisable()
        {
            if (_attachedSoundBox == null)
            {
                return;
            }
            
        }

        protected override void PrepareSoundCustom(SoundBox sb)
        {
           
            
        }
        
    }

    [System.Serializable]
    public class SoundFadeSettings
    {
        [Tooltip("Fade Curve, where horizontal axis is the timeProgression and the vertical axis is the volume")]
        public AnimationCurve fadeCurve = AnimationCurve.Constant(0,1,1);
        public float fadeDuration = 0;
    }
}
