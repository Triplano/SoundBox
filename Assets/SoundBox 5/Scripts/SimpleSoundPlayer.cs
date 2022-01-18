using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Triplano.Assets.SoundBox5
{

	public class SimpleSoundPlayer : MonoBehaviour
	{
		private enum PlayAt
		{
			ENABLE,
			START
		}

		[SerializeField]
		private bool _setThisAsParent = false;
		[SerializeField]
		private SB_Config_Base _soundToPlay;
		[SerializeField]
		private PlayAt _playAt = PlayAt.ENABLE;
		[SerializeField]
		private float _minDelay = 0;
		[SerializeField]
		private float _maxDelay = 0;

		private void OnEnable()
		{
			if (_playAt == PlayAt.ENABLE)
			{
				PlaySound();
			}
		}

		private void Start()
		{
			if (_playAt == PlayAt.START)
			{
				PlaySound();
			}
		}

		private void PlaySound()
		{
			if (_soundToPlay == null)
			{
				return;
			}

			StartCoroutine(PlaySoundCoroutine());

			IEnumerator PlaySoundCoroutine()
			{
				float delay = Random.Range(_minDelay, _maxDelay);

				yield return new WaitForSeconds(delay);

				if (_setThisAsParent)
				{
					SoundBox.PlaySound(_soundToPlay, transform.position, transform);
				}
				else
				{
					SoundBox.PlaySound(_soundToPlay, transform.position);
				}

			}

		}
	}

}