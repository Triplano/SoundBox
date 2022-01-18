using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

namespace Triplano.Assets.SoundBox5
{
    
    public enum SBState {NONE,PREPARED,PLAYING,LOOP,STOP,PAUSED,FINISH}
    
    [RequireComponent(typeof(AudioSource))]
    public class SoundBox : MonoBehaviour
    {
        public delegate void OnSetNewSourceHandler();
        public OnSetNewSourceHandler onSetNewSource;
        
        public delegate void OnSetNewConfigHandler();
        public OnSetNewConfigHandler onSetNewConfig;
        
        public delegate void SoundBoxStateChangeHandler();
        public SoundBoxStateChangeHandler OnStartPlaying;
        public SoundBoxStateChangeHandler OnPause;
        public SoundBoxStateChangeHandler OnResume;
        public SoundBoxStateChangeHandler OnStop;
        public SoundBoxStateChangeHandler OnFinishPlaying;
  
        
        private SBState _currenState;
        private AudioSource _source;
        private SoundBoxPooler _pooler;
        private SB_Config_Base _config;
        private bool _isPlaying;
        private bool _finishLoading;
        private float _clipDuration;
        private bool _returnToPoolAfterFinish = false;
        public void Initialize(SoundBoxPooler pooler = null)
        {
            if (pooler != null)
            {
                _pooler = pooler; 
            }
   
            _source = GetComponent<AudioSource>();
            _source.playOnAwake = false;
            _finishLoading = false;
            _isPlaying = false;
            _returnToPoolAfterFinish = true;
            UpdateState(SBState.NONE);
        }
        
        public static void PlaySound(SB_Config_Base newConfig, Vector3 position, Transform parent = null, SoundBox SbToUse = null)
        {
            SoundBox sb;
            
            if (SbToUse == null)
            {
                 sb = SoundBoxPooler.sInstance.GetAvaliableSource();
            }
            else
            {
                sb = SbToUse;
            }

            sb._config = Instantiate(newConfig);
            sb.transform.position = position;
            
            if (parent != null)
            {
                sb.transform.SetParent(parent);
            }

            sb._config.PrepareSoundBox(sb);
            //sb = SB_PrepareLogic.Prepare(newConfig, sb);
            sb.UpdateState(SBState.PREPARED);
            sb.gameObject.SetActive(true);
            sb.Play();

          
            
            if (sb._config is SB_Config_Music_Fade)
            {
                SB_Config_Music_Fade sB_Config_Music_Fade = sb._config as SB_Config_Music_Fade;

                sb.StartFadeSound(sB_Config_Music_Fade.PitchFade(sb));
            }
            
   
        }
        
        public void Stop()
        {
            StopAllCoroutines();
            
            if (_source)
            {
                _source.Stop();
            }
            else
            {
                Initialize();
                _source.Stop();
            }
            
            UpdateState(SBState.STOP);
        }

        public void Pause(bool state)
        {
            if (state == true)
            {
                if (_isPlaying)
                {
                    _source.Pause();
                    Play();
                    _isPlaying = false;
                    UpdateState(SBState.PAUSED);
                }
            }
            else
            {
                StartCoroutine(Resume());

                IEnumerator Resume()
                {
                    _source.UnPause();
                    _isPlaying = true;
                    UpdateState(SBState.PLAYING);
                    yield return new WaitForSeconds(_clipDuration - _source.time);
                    FinishPlaying();
                    
                }
            }
            
        }
        
        private void OnEnable()
        {
            Initialize();
        }
        
        private void UpdateState(SBState newState)
        {
            switch (newState)
            {
                case SBState.PLAYING:
                    if (_currenState == SBState.PAUSED)
                    {
                        OnResume?.Invoke();
                    }
                    else
                    {
                        OnStartPlaying?.Invoke();
                    }
                    break;
                case SBState.PAUSED:
                    OnPause?.Invoke();
                    break;
                case SBState.STOP:
                    OnPause?.Invoke();
                    break;
                case SBState.FINISH:
                    OnFinishPlaying?.Invoke();
                    break;
            }
            
            _currenState = newState;

        }

        private void Play()
        {

            StartCoroutine(PlaySoundCoroutine());
            
            IEnumerator PlaySoundCoroutine()
            {
                _source.transform.name = $" Source of Clip: {_source.clip.name}";

                _isPlaying = true;
                _clipDuration = _source.clip.length / Mathf.Abs(_source.pitch); // we calculate the Duration right before playing, to have a more accurate value, since this  could be changed by some module.

                UpdateState(SBState.PLAYING);
                _source.gameObject.hideFlags = HideFlags.None;
                _source.Play();
                _finishLoading = false;

                yield return new WaitForSeconds(_clipDuration);
                FinishPlaying();
            }
        }
        
        private void FinishPlaying()
        {
            
            _finishLoading = false;
            _isPlaying = false;
            UpdateState(SBState.FINISH);
            
            if (_source.loop)
            {
                _isPlaying = true;
            }
            else if (_returnToPoolAfterFinish && _pooler != null)
            {
                transform.SetParent(_pooler.transform);
                gameObject.SetActive(false);
                _pooler.ReturnAudioSource(this);
            }
        }

        private void StartFadeSound (IEnumerator pitchFade)
        {
            StartCoroutine(pitchFade);
        }
        
        public bool IsPlaying => _isPlaying;

        public AudioSource Source
        {
            get => _source;
            set
            {
                _source = value;
                onSetNewSource?.Invoke();
            }
        }
        
        public AudioClip Clip
        {
            set
            {
                _source.clip = value;
                
                if (_source)
                {
                    _source.clip = value;
                }
            }
        }

        public float Pitch
        {
            set
            {
                _source.pitch = value;

                if (_source)
                {
                    _source.pitch = value;
                }
            }
            get
            {
                if (_source)
                {
                    return _source.pitch;
                }
                return 0;
            }
        }
        public float Volume
        {
            
            set             
            {
                if (_source)
                {
                    _source.volume = value;
                }
            }
            
            get
            {
                if (_source)
                {
                    return _source.volume;
                }
                return 0;
            }
            
        }


        public float Duration()
        {
            if (_source)
            {
                return (_source.clip.length);
            }

            return 0;
        }

        public float RemaningDuration()
        {
            if (_source)
            {
                return (_source.clip.length - _source.time);
            }

            return 0;
        }

        public void SetOutputGroup(AudioMixerGroup group)
        {
            _source.outputAudioMixerGroup = group;
        }

        public bool ReturnToPoolAfterFinish
        {
            get { return _returnToPoolAfterFinish; }
            set { _returnToPoolAfterFinish = value; }
        }

    }
}
