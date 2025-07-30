using System;
using UnityEngine;

namespace ToB
{
    public class AnimatorInitializeOnEnable : MonoBehaviour
    {
        Animator animator;
        private void Awake()
        {
            animator = GetComponent<Animator>();
            if(!animator) Debug.LogError("Animator is null");
        }
        
        private void OnEnable()
        {
            animator.Rebind();
        }
    }
}
