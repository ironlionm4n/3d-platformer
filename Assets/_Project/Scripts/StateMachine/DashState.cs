using Platformer._Project.Scripts;
using UnityEngine;

namespace Platformer
{
    public class DashState : BaseState
    {
        public DashState(PlayerController playerController, Animator animator) : base(playerController, animator)
        {
        }
        
        public override void OnEnter()
        {
            animator.CrossFade(DashHash, crossFadeDuration);
        }

        public override void FixedUpdate()
        {
            playerController.HandleMovement();
        }
    }
}