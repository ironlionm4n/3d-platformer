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
        [SerializeField] private float gravityMultiplier = 3f;
        [Header("Dash Settings")]
        [SerializeField] private float dashForce = 10f;
        [SerializeField] private float dashDuration = 1f;
        [SerializeField] private float dashCooldown = 2f;
        
        private Transform mainCam;
        private const float ZeroF = 0;
        private float currentSpeed;
        private float velocity;
        private float jumpVelocity;
        private float dashVelocity = 1f;
        
        private Vector3 movement;
        private List<Timer> timers;
        private CountdownTimer jumpTimer;
        private CountdownTimer jumpCooldownTimer;
        private CountdownTimer dashTimer;
        private CountdownTimer dashCooldownTimer;

        private StateMachine stateMachine;
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
            dashTimer = new CountdownTimer(dashDuration);
            dashCooldownTimer = new CountdownTimer(dashCooldown);
            jumpTimer.OnTimerStart += () => jumpVelocity = jumpForce;
            jumpTimer.OnTimerStop += () => jumpCooldownTimer.Start();
            dashTimer.OnTimerStart += () => dashVelocity = dashForce;
            dashTimer.OnTimerStop += () =>
            {
                dashVelocity = 1f;
                dashCooldownTimer.Start();
            };
            timers = new (4) { jumpTimer, jumpCooldownTimer, dashTimer, dashCooldownTimer };
            
            // State Machine
            stateMachine = new StateMachine();
            // Declare states
            var locomotionState = new LocomotionState(this, animator);
            var jumpState = new JumpState(this, animator);
            var dashState = new DashState(this, animator);
            
            // Declare transitions
            At(locomotionState, jumpState, new FuncPredicate(()=> jumpTimer.IsRunning));
            At(locomotionState, dashState, new FuncPredicate(()=> dashTimer.IsRunning));
            Any(locomotionState, new FuncPredicate(()=> !jumpTimer.IsRunning && groundCheck.IsGrounded && !dashTimer.IsRunning));
            
   
            
            // Set initial state
            stateMachine.SetState(locomotionState);
        }

        void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
        void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

        private void OnEnable()
        {
            input.Jump += OnJump;
            input.Dash += OnDash;
        }

        private void OnDisable()
        {
            input.Jump -= OnJump;
            input.Dash -= OnDash;
        }

        private void Start() => input.EnablePlayerActions();

        private void Update()
        {
            movement = new Vector3(input.Direction.x, 0f, input.Direction.y);
            stateMachine.Update();
            HandleTimers();
            UpdateAnimator();
        }
        
        private void FixedUpdate()
        {
            stateMachine.FixedUpdate();
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

        private void OnDash(bool performed)
        {
            if(performed && !dashTimer.IsRunning && !dashCooldownTimer.IsRunning)
            {
                // Start dash
                dashTimer.Start();
            }
            else if(!performed && dashTimer.IsRunning)
            {
                // Stop dash
                dashTimer.Stop();
            }
        }
        
        public void HandleJump()
        {
            // On ground and not jumping, reset velocity to zero and timer
            if (!jumpTimer.IsRunning && groundCheck.IsGrounded)
            {
                jumpVelocity = ZeroF;
                jumpTimer.Stop();
                return;
            }
            
            // if jumping or falling calculate velocity
            if (!jumpTimer.IsRunning)
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

        public void HandleMovement()
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
            var velocity = adjustedDirection * (speed * dashVelocity * Time.fixedDeltaTime);
            rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);
        }

        private void HandleRotation(Vector3 adjustedDirection)
        {
            var targetRotation = Quaternion.LookRotation(adjustedDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            //transform.LookAt(transform.position + adjustedDirection);
        }

        private void SmoothSpeed(float value)
        {
            currentSpeed = Mathf.SmoothDamp(currentSpeed, value, ref velocity, smoothTime);
        }
    }
}