using Platformer._Project.Scripts;
using UnityEngine;

namespace Platformer
{
    public abstract class BaseState : IState
    {
        protected readonly PlayerController playerController;
        protected readonly Animator animator;
        
        protected static readonly int LocomotionHash = Animator.StringToHash("Locomotion");
        protected static readonly int JumpHash = Animator.StringToHash("Jump");
        protected static readonly int DashHash = Animator.StringToHash("Dash");
        
        protected const float crossFadeDuration = 0.1f;
        
        protected BaseState(PlayerController playerController, Animator animator)
        {
            this.playerController = playerController;
            this.animator = animator;
        }
        
        public virtual void OnEnter()
        {
            // noop
        }

        public virtual void Update()
        {
            // noop
        }

        public virtual void FixedUpdate()
        {
            // noop
        }

        public virtual void OnExit()
        {
            Debug.Log("BaseState.OnExit()");
            // noop
        }
    }
}