using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tir
{
    [RequireComponent(typeof(AudioSource))]
    public class Tir_SoundManager : MonoBehaviour
    {
        [System.Serializable]
        public struct AudioClipExtended
        {
            public AudioClip Clip;
            [Range(0, 1)] public float Volume;
        }

        public AudioSource Source => _source;
        AudioSource _source;

        [SerializeField, Range(0, 100)]
        int[] _musicTransitionPourcent = new int[2];
        
        [SerializeField]
        AudioClipExtended[] _backgroundMusic;

        [SerializeField]
        AudioClipExtended[] _impactClip;

        [SerializeField]
        AudioSource[] _impactSource;

        [SerializeField]
        AudioClipExtended _endSound;

        int _lastMusicId = -1;
        bool _backgroundMusicIsPlaying = false;

        public void Start()
        {
            _source = GetComponent<AudioSource>();
            Tir_GameManager.Instance.OnGameStart += () => _backgroundMusicIsPlaying = true;
            Tir_GameManager.Instance.OnGameStart += () => StartCoroutine(PlayBackGroundSound());
            Tir_GameManager.Instance.OnGameEnd += StopMusic;
        }

        public void StopMusic()
        {
            Tir_GameManager.Instance.OnGameEnd -= StopMusic;
            _backgroundMusicIsPlaying = false;
            Source.Stop();
            Source.clip = null;
            Source.volume = 0;
            PlayFinalSound();
        }

        private void PlayFinalSound()
        {
            Source.clip = _endSound.Clip;
            Source.volume = _endSound.Volume;
            Source.loop = false;
            Source.Play();
        }

        public IEnumerator PlayBackGroundSound()
        {
            if (!_backgroundMusicIsPlaying)
                yield break;

            int GetMusicId(float pourcent)
            {
                if (pourcent <= _musicTransitionPourcent[0])
                        return 0;

                if (pourcent <= _musicTransitionPourcent[1])
                    return 1;
                return 2;
            }

            yield return null;

            var currentTime = Tir_GameManager.Instance.TimerManager.CurrentTime;
            var totalTime = Tir_GameManager.Instance.TimerManager.Timer;
            float pourcent = (float)currentTime.TotalSeconds / totalTime * 100;
            int id = GetMusicId(pourcent);
            //Debug.Log(id + " | " + (float)currentTime.TotalSeconds + " / " + totalTime);

            if (id == _lastMusicId && Source.isPlaying)
            {
                yield return null;
                StartCoroutine(PlayBackGroundSound());
                yield break;
            }

            _lastMusicId = id;
            Source.clip = _backgroundMusic[id].Clip;
            Source.volume = _backgroundMusic[id].Volume;
            Source.Play();

            yield return null;

            StartCoroutine(PlayBackGroundSound());
        }

        public void PlayImpactSound(TirTeam team)
        {
            _impactSource[team.Id].clip = _impactClip[team.Id].Clip;
            _impactSource[team.Id].volume = _impactClip[team.Id].Volume;
            _impactSource[team.Id].Play();
        }
    }
}