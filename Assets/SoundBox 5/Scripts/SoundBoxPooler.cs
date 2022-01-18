using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Triplano.Assets.SoundBox5
{

    public class SoundBoxPooler : MonoBehaviour
    {
        [SerializeField]
        private SoundBox _audioSourcePrefab;
        [SerializeField]
        private int _initialSize = 50;
        [SerializeField]
        private Queue<SoundBox> _audioSources = new Queue<SoundBox>();

        public static SoundBoxPooler sInstance;
        [SerializeField]
        private List<SoundBox> _avaliableSourcesDebug = new List<SoundBox>();

        private void Awake()
        {
            if (sInstance != null)
            {
                Destroy(gameObject);
                return;
            }
            
            sInstance = this;
            DontDestroyOnLoad(sInstance);

            CreateAudioSourcePools();

        }
      
        public void ReturnAudioSource( SoundBox sb)
        {
            
            //sb.gameObject.hideFlags = HideFlags.HideInHierarchy;
            _audioSources.Enqueue(sb);
            _avaliableSourcesDebug.Add(sb);

        }
        public SoundBox GetAvaliableSource()
        {
            SoundBox source;
         
            if (_audioSources.Count > 0)
            {
                source = _audioSources.Dequeue();
                _avaliableSourcesDebug.RemoveAt(0);
         
            }
            else
            {
                source = CreateNewSource();
            }

            return source;
        }

        private void CreateAudioSourcePools()
        {

            for (int i = 0; i < _initialSize; i++)
            {
                SoundBox newSource = CreateNewSource();
                _audioSources.Enqueue(newSource);
                _avaliableSourcesDebug.Add(newSource);
                newSource.transform.SetParent(transform);
                //newSource.gameObject.hideFlags = HideFlags.HideInHierarchy;
            }

        }

        private SoundBox CreateNewSource()
        {
            SoundBox newSoundBox = Instantiate(_audioSourcePrefab);
            newSoundBox.gameObject.SetActive(false);
            newSoundBox.Initialize(this);
         
            return newSoundBox;
        }
    }
}
