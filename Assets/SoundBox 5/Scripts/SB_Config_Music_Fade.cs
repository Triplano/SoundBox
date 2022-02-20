using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Triplano.Assets.SoundBox5
{
    [CreateAssetMenu(fileName = "New Music Fade Sound", menuName = "Audio/Music Fade")]
    public class SB_Config_Music_Fade : SB_Config_Base
    {
        [Header("FADE SETTINGS")]
        public MusicFadeType musicFadeType = MusicFadeType.INCREASE;

        public float initialPitch = 1.0f;

        public float timeFadingPitch = 1.0f;
        public float addPitchOverTime = 0.2f;

        protected override void PrepareSoundCustom(SoundBox sb)
        {
            
            sb.ReturnToPoolAfterFinish = false;
            sb.Source.loop = true;
            sb.Pitch = initialPitch;
        }

        public IEnumerator PitchFade (SoundBox sb)
        {
            float currentTimeFading = 0;

            while (currentTimeFading < timeFadingPitch)
            {
                currentTimeFading += Time.deltaTime;

                switch (musicFadeType)
                {
                    case MusicFadeType.INCREASE:
                        sb.Pitch = initialPitch + addPitchOverTime * currentTimeFading;
                        break;

                    case MusicFadeType.DECREASE:
                        sb.Pitch = initialPitch - addPitchOverTime * currentTimeFading;
                        break;

                    default:
                        break;
                }

                yield return null;
            }
        }
    }
}