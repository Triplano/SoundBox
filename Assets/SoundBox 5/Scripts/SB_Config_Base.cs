using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace Triplano.Assets.SoundBox5

{
	public abstract class SB_Config_Base : ScriptableObject
	{
		[Tooltip("The list of soundclips used in this settings. Note that for default a random clip from this list will be selected to play everytime the SB plays a sound" +
		         "you can change by checking 'ChangeClipsOnce' in where this config file is being used.")]
		public List<AudioClip> audioClips;

		[Space(20)]
		[Tooltip("for default, if the min and max pitch are different, than it will be randomized. Otherwise, it will use the value of the MaxPitch")]
		public float minPitch = 0.9f, maxPitch = 1.1f;

		[Space(10)]
		[Tooltip("The AudioMixer output group that this sound will Play. If empty, the group will be the one used in the AudioSource")]
		public AudioMixerGroup outputGroup;
		[Space(5)]
		[Tooltip("0 for 2D sound and 1 for 3D sound")]
		[Range(0,1)]
		public float spatialBlend = 1;
		[Header("3D sound Settings")]
		[Range(0,5)]
		public float dopplerEffect = 1;
		[Range(0,360)]
		public float spread = 80;
		public AnimationCurve volumeRollOffCurve = AnimationCurve.EaseInOut(0,1,1,0);
		public float minDistance = 4;
		public float maxDistance = 20;

		public void PrepareSoundBox(SoundBox sb)
		{
			sb.Clip = audioClips[Random.Range(0, audioClips.Count)];
			sb.Pitch = Random.Range(minPitch, maxPitch);
			sb.SetOutputGroup(outputGroup);
			sb.Source.spatialBlend = spatialBlend;
			sb.Source.dopplerLevel = dopplerEffect;
			sb.Source.spread = spread;
			sb.Source.SetCustomCurve(AudioSourceCurveType.CustomRolloff,volumeRollOffCurve);
			sb.Source.minDistance = minDistance;
			sb.Source.maxDistance = maxDistance;
			PrepareSoundCustom(sb);
		}

		protected abstract void PrepareSoundCustom(SoundBox sb);
	}

}