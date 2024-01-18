using Platformer._Project.Scripts;
using UnityEngine;

namespace Platformer
{
    public class JumpState : BaseState
    {
        public JumpState(PlayerController playerController, Animator animator) : base(playerController, animator)
        {
        }

        public override void OnEnter()
        {
            Debug.Log("JumpState.OnEnter()");
            animator.CrossFade(JumpHash, crossFadeDuration);
        }
        
        public override void FixedUpdate()
        {
            // Call playerController.Jump() here and move logic
            playerController.HandleJump();
            playerController.HandleMovement();
        }
    }
}