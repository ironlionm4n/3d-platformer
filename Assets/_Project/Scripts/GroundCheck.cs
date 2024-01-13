using UnityEngine;

namespace Platformer._Project.Scripts
{
    public class GroundCheck : MonoBehaviour
    {
        [SerializeField] private float groundDistance = 0.08f;
        [SerializeField] private float radius = 1f;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private Vector3 sphereCastOriginOffset = new (0f, .1f, 0f);

        public bool IsGrounded { get; private set; }

        private void FixedUpdate()
        {
            //IsGrounded = Physics.SphereCast(transform.position + sphereCastOriginOffset , radius, Vector3.down, out _, groundDistance, groundMask);
            IsGrounded = Physics.CheckSphere(transform.position, radius, groundMask);
        }
        
        // Draw gizmos to help visualize the sphere cast with different colors depending on if we are grounded or not with a line for the cast direction
        private void OnDrawGizmos()
        {
            Gizmos.color = IsGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(transform.position + sphereCastOriginOffset, radius);
            Gizmos.DrawLine(transform.position + sphereCastOriginOffset, transform.position + sphereCastOriginOffset + Vector3.down * groundDistance);
        }
    }
}