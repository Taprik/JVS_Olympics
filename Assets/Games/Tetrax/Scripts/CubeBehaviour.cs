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

        public void OnClick() => OnClick(false);

        public void OnClick(bool onSpe)
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

            if (Data.IsSpe && !onSpe) Spe();

            Destroy(this.gameObject, 0.5f);
        }

        private void Spe()
        {
            var team = Tetrax_GameManager.Instance.Teams.ToList().Find(t => t.Color == Data.Color);
            var list = new List<CubeBehaviour>();
            foreach (var cube in team.CubesList)
            {
                if (Vector2.Distance(cube.transform.position, transform.position) <= 200)
                {
                    if(cube.TryGetComponent(out CubeBehaviour cb))
                    {
                        list.Add(cb);
                    }
                }
            }

            for (int i = 0; i < list.Count; i++)
            {
                list[i].OnClick(true);
            }
        }

        public void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, 70);
        }
    }
}