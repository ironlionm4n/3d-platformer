using System;
using System.Collections.Generic;
using Cinemachine;
using KBCore.Refs;
using Platformer._Project.Scripts.Input;
using Platformer._Project.Scripts.Utilities;
using UnityEngine;

namespace Platformer._Project.Scripts
{
    public class PlayerController : ValidatedMonoBehaviour
    {
        [Header("References")] 
        [SerializeField, Self] private Rigidbody rb;
        [SerializeField, Self] private GroundCheck groundCheck;
        [SerializeField, Self] private Animator animator;
        [SerializeField, Anywhere] CinemachineFreeLook freeLookVCam;
        [SerializeField, Anywhere] InputReader input;
        
        
        [Header("Movement Settings")]
        [SerializeField] private float speed = 6f;
        [SerializeField] private float rotationSpeed = 15f;
        [SerializeField] private float smoothTime = 0.2f;

        [Header("Jump Settings")]
        [SerializeField] private float jumpForce = 10f;
        [SerializeField] private float jumpDuration = 0.5f;
        [SerializeField] private float jumpCooldown = 0f;
        [SerializeField] private float jumpMaxHeight = 2f;
        [SerializeField] private float gravityMultiplier = 3f;
        
        private Transform mainCam;
        private const float ZeroF = 0;
        private float currentSpeed;
        private float velocity;
        private float jumpVelocity;
        private Vector3 movement;
        private List<Timer> timers;
        private CountdownTimer jumpTimer;
        private CountdownTimer jumpCooldownTimer;
        
        private static readonly int Speed = Animator.StringToHash("Speed");

        private void Awake()
        {
            mainCam = Camera.main.transform;
            freeLookVCam.Follow = transform;
            freeLookVCam.LookAt = transform;
            freeLookVCam.OnTargetObjectWarped(transform,
                transform.position - freeLookVCam.transform.position - Vector3.forward);
            rb.freezeRotation = true;
            
            // Setup timers
            jumpTimer = new CountdownTimer(jumpDuration);
            jumpCooldownTimer = new CountdownTimer(jumpCooldown);
            timers = new (2) { jumpTimer, jumpCooldownTimer };
            
            jumpTimer.OnTimerStop += () => jumpCooldownTimer.Start();
        }

        private void OnEnable()
        {
            input.Jump += OnJump;
        }



        private void OnDisable()
        {
            input.Jump -= OnJump;
        }

        private void Start() => input.EnablePlayerActions();

        private void Update()
        {
            movement = new Vector3(input.Direction.x, 0f, input.Direction.y);

            HandleTimers();
            UpdateAnimator();
        }
        
        private void FixedUpdate()
        {
            HandleJump();
            HandleMovement();
        }

        private void HandleTimers()
        {
            foreach (var timer in timers)
            {
                timer.Tick(Time.deltaTime);
            }
        }

        private void OnJump(bool performed)
        {
            if(performed && !jumpTimer.IsRunning && !jumpCooldownTimer.IsRunning && groundCheck.IsGrounded)
            {
                // Start jump
                jumpTimer.Start();
            }
            else if(!performed && jumpTimer.IsRunning)
            {
                // Stop jump
                jumpTimer.Stop();
            }
        }
        
        private void HandleJump()
        {
            // On ground and not jumping, reset velocity to zero and timer
            if (!jumpTimer.IsRunning && groundCheck.IsGrounded)
            {
                jumpVelocity = ZeroF;
                jumpTimer.Stop();
                return;
            }
            
            // if jumping or falling calculate velocity
            if (jumpTimer.IsRunning)
            {
                // Progress point for initial burst of velocity
                float launchPoint = 0.9f;
                if (jumpTimer.Progress > launchPoint)
                {
                    // Calculate the velocity required to reach the jump height using physics equations v = sqrt(2gh)
                    jumpVelocity = Mathf.Sqrt(2 * jumpMaxHeight * Mathf.Abs(Physics.gravity.y));
                }
                else
                {
                    // Gradually apply less velocity as jump progresses
                    jumpVelocity += (1-jumpTimer.Progress) * jumpForce * Time.deltaTime;
                }
            }
            else
            {
                // Gravity takes over
                jumpVelocity += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
            }

            // Apply velocity
            rb.velocity = new Vector3(rb.velocity.x, jumpVelocity, rb.velocity.z);
        }

        private void UpdateAnimator()
        {
            animator.SetFloat(Speed, currentSpeed);
        }

        private void HandleMovement()
        {
            // Rotate movement direction to match camera rotation
            var adjustedDirection = Quaternion.AngleAxis(mainCam.eulerAngles.y, Vector3.up) * movement;
            if (adjustedDirection.magnitude > ZeroF)
            {
                HandleRotation(adjustedDirection);
                HandleHorizontalMovement(adjustedDirection);
                SmoothSpeed(adjustedDirection.magnitude);
            }
            else
            {
                SmoothSpeed(ZeroF);
                // Reset horizontal velocity for a snappy stop
                rb.velocity = new Vector3(ZeroF, rb.velocity.y, ZeroF);
            }
        }

        private void HandleHorizontalMovement(Vector3 adjustedDirection)
        {
            // Move the player
            var velocity = adjustedDirection * (speed * Time.fixedDeltaTime);
            rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);
        }

        private void HandleRotation(Vector3 adjustedDirection)
        {
            var targetRotation = Quaternion.LookRotation(adjustedDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            transform.LookAt(transform.position + adjustedDirection);
        }

        private void SmoothSpeed(float value)
        {
            currentSpeed = Mathf.SmoothDamp(currentSpeed, value, ref velocity, smoothTime);
        }
    }
}