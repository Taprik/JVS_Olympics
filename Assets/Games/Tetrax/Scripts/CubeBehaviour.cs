using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tool;
using UnityEngine;

namespace Tetrax
{
    public class CubeBehaviour : MonoBehaviour
    {
        [SerializeField] AudioClip[] _onDestroyAudio;
        bool IsDestroy = false;
        public CubeData Data { get; set; }

        public void OnClick()
        {
            var team = Tetrax_GameManager.Instance.Teams.ToList().Find(t => t.Color == Data.Color);
            if (IsDestroy || team.IsGameOver) return;
            IsDestroy = true;

            var animator = GetComponent<Animator>();
            animator.SetTrigger("Hit");

            team.CubesList.Remove(this.gameObject);
            team.Score += Data.Score;
            team.ScoreText.text = team.Score.ToString("00");

            var source = GetComponent<AudioSource>();
            source.clip = _onDestroyAudio.RandomElement();
            source.Play();

            Destroy(this.gameObject, 0.5f);
        }
    }
}