using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tetrax
{
    public class CubeBehaviour : MonoBehaviour
    {
        bool IsDestroy = false;

        public void OnClick()
        {
            if (IsDestroy) return;
            IsDestroy = true;

            var animator = GetComponent<Animator>();
            animator.SetTrigger("Hit");
        }
    }
}