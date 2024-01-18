using Platformer._Project.Scripts;
using UnityEngine;

namespace Platformer
{
    public class LocomotionState : BaseState
    {
        public LocomotionState(PlayerController playerController, Animator animator) : base(playerController, animator)
        {
        }

        public override void OnEnter()
        {
            Debug.Log("LocomotionState.OnEnter()");
            animator.CrossFade(LocomotionHash, crossFadeDuration);
        }
        
        public override void FixedUpdate()
        {
            // Call playerController.Move() here and move logic
            playerController.HandleMovement();
        }
    }
}