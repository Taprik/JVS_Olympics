using System.Collections;
using System.Collections.Generic;
using Tool;
using UnityEngine;

namespace Target
{
    public class Target_SoundManager : MonoBehaviour
    {
        public static Target_SoundManager Instance;

        public void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public AudioSource Source => source;
        [SerializeField] AudioSource source;
        [SerializeField] AudioClip[] TargetBreakSound;
        [SerializeField] AudioClip[] EndSound;

        public void PlayTargetBreakSound()
        {
            Source.clip = TargetBreakSound.RandomElement();
            Source.Play();
        }

        public void PlayEndSound()
        {
            Source.clip = EndSound.RandomElement();
            Source.Play();
        }
    }
}