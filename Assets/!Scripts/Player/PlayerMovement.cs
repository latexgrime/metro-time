using System.Collections;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace NALEO._Scripts.Player
{
    /// <summary>
    ///     Script in charge of handling the input for the movement of the player.
    /// </summary>
    public class PlayerMovement : MonoBehaviour
    {
        private PlayerAudio _playerAudio;
        private InputManager _inputManager;
        private AudioSource _audioSource;
        private Rigidbody _rb;

        [Header("- Movement")] 
        [SerializeField] private float walkSpeed = 7f;
        [SerializeField] private float groundDrag = 7.5f;
        private Vector3 _moveDirection;
        private float _horizontalInput;
        private float _verticalInput;
        [SerializeField] private float _moveSpeed;

        [Header("- Jumping")] 
        [SerializeField] private float jumpForce = 5f;
        [SerializeField] private float jumpCooldown = 0.5f;
        [SerializeField] private float airMultiplier = 0.75f;

        [Header("- Running")] 
        [SerializeField] private float sprintSpeed = 14f;
        
        [Header("- Crouching")] 
        [SerializeField] private float crouchSpeed;
        [SerializeField] private float crouchYScale;
        private float _defaultYScale;
        
        [Header("- Dash Settings")]
        [SerializeField] private float dashSpeed = 20f;
        [SerializeField] private float dashDuration = 0.2f;
        [SerializeField] private float dashCooldown = 1f;
        
        [Header("- Ground check")] 
        [SerializeField] public LayerMask GroundLayer;
        [SerializeField] private Transform camOrientation;
        [SerializeField] public float playerHeight = 1f;

        [Header("- Slope handling")] 
        [SerializeField] private float maxSlopeAngle;
        private RaycastHit _slopeHit;

        // Flags.
        private bool _readyToJump;
        private bool _isDashing = false;
        private bool _canDash = true;
        private bool _exitingSlope;
        private bool _grounded;
        private bool _wasGrounded;

        private PlayerState _currentState;
        public PlayerState CurrentState => _currentState;
        private PlayerState _previousState;
        
        private void Start()
        {
            InitializeScript();
            TransitionToState(PlayerState.Idle);
        }

        private void InitializeScript()
        {
            _rb = GetComponent<Rigidbody>();
            _audioSource = GetComponent<AudioSource>();
            _inputManager = GetComponent<InputManager>();
            _playerAudio = FindFirstObjectByType<PlayerAudio>();

            _rb.freezeRotation = true;
            _readyToJump = true;
            _defaultYScale = transform.localScale.y;
            _moveSpeed = walkSpeed;
        }

        private void Update()
        {
            GroundCheck();
            HandleStateTransitions();
            SpeedControl();
            HandleDrag();

            _wasGrounded = _grounded;
        }

        private void FixedUpdate()
        {
            MovePlayer();
        }
        
        private void HandleStateTransitions()
        {
            if (_inputManager.jumpInput && _readyToJump && _grounded)
            {
                _readyToJump = false;
                TransitionToState(PlayerState.Jumping);
                return;
            }
            
            if (_inputManager.dashInput && _canDash && !_isDashing)
            {
                TransitionToState(PlayerState.Dashing);
                return;
            }
            
            switch (_currentState)
            {
                case PlayerState.Idle:

                    if (!_grounded)
                        TransitionToState(PlayerState.InAir);

                    else if (_inputManager.horizontalInput != 0 || _inputManager.verticalInput != 0)
                        TransitionToState(PlayerState.Walking);
                    
                    else if (_inputManager.sprintInput && (_inputManager.horizontalInput > 0 || _inputManager.verticalInput > 0))
                        TransitionToState(PlayerState.Sprinting);
                    
                    else if (_inputManager.crouchInput)
                        TransitionToState(PlayerState.Crouching);
                    break;
                
                case PlayerState.Walking:
                    if (!_grounded)
                        TransitionToState(PlayerState.InAir);
                    
                    else if (_inputManager.sprintInput && (_inputManager.horizontalInput > 0 || _inputManager.verticalInput > 0))
                        TransitionToState(PlayerState.Sprinting);
                    
                    else if (_inputManager.crouchInput)
                        TransitionToState(PlayerState.Crouching);
                    
                    else if (_inputManager.horizontalInput == 0 && _inputManager.verticalInput == 0)
                        TransitionToState(PlayerState.Idle);
                    break;
                
                case PlayerState.Sprinting:
                    if (!_grounded)
                        TransitionToState(PlayerState.InAir);
                    
                    else if (!_inputManager.sprintInput && (_inputManager.horizontalInput > 0 || _inputManager.verticalInput > 0))
                        TransitionToState(PlayerState.Walking);
                    
                    else if (_inputManager.crouchInput)
                        TransitionToState(PlayerState.Crouching);
                    
                    else if (_inputManager.horizontalInput == 0 && _inputManager.verticalInput == 0)
                        TransitionToState(PlayerState.Idle);
                    break;
 
                case PlayerState.Crouching:
                    if (!_grounded)
                        TransitionToState(PlayerState.InAir);
                    
                    else if (!_inputManager.crouchInput && !_inputManager.sprintInput && 
                             (_inputManager.horizontalInput > 0 || _inputManager.verticalInput > 0))
                        TransitionToState(PlayerState.Walking);
                    
                    else if (!_inputManager.crouchInput && _inputManager.sprintInput)
                        TransitionToState(PlayerState.Sprinting);
                    
                    else if (!_inputManager.crouchInput && 
                             _inputManager.horizontalInput == 0 && _inputManager.verticalInput == 0)
                        TransitionToState(PlayerState.Idle);
                    
                    break;
                
                case PlayerState.Jumping:
                    TransitionToState(PlayerState.InAir);
                    break;
                
                case PlayerState.Dashing:
                    if (_grounded)
                        TransitionToState(PlayerState.Walking);
                    break;
                
                case PlayerState.InAir:
                    if (_grounded)
                        TransitionToState(PlayerState.Walking);
                    break;
            }
        }

        private void TransitionToState(PlayerState newState)
        {
            PlayerState oldState = _currentState;
            
            OnExitState(_currentState);
            _playerAudio.PlayStateExitSound(_currentState);

            _currentState = newState;

            OnEnterState(newState);
            _playerAudio.PlayStateEnterSound(newState);
        }
        
        private void OnEnterState(PlayerState state)
        {
            switch (state)
            {
                case PlayerState.Idle:
                    // Nothing.
                    break;
            
                case PlayerState.Walking:
                    Walk();
                    break;
            
                case PlayerState.Sprinting:
                    Sprint();
                    break;
            
                case PlayerState.Crouching:
                    Crouch();
                    break;
            
                case PlayerState.Jumping:
                    Jump();
                    break;
            
                case PlayerState.Dashing:
                    StartCoroutine(DashCoroutine());
                    break;
            }
        }

        private void OnExitState(PlayerState state)
        {
            switch (state)
            {
                case PlayerState.Crouching:
                    transform.localScale = new Vector3(transform.localScale.x, _defaultYScale, transform.localScale.z);
                    break;
            
                case PlayerState.Jumping:
                    _exitingSlope = false;
                    break;
            
                case PlayerState.Dashing:
                    // Any cleanup needed after dashing
                    break;
            }
        }
        
        private void GroundCheck()
        {
            _grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 1.1f, GroundLayer);
        }

        public bool IsGrounded()
        {
            if (Physics.Raycast(transform.position, Vector3.down, playerHeight * 1.1f, GroundLayer))
            {
                return true;
            }
            return false;
        }

        private void HandleDrag()
        {
            if (_grounded)
                _rb.linearDamping = groundDrag;
            else
                _rb.linearDamping = 0;
        }

        private void Crouch()
        {
            _moveSpeed = crouchSpeed;
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            _rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        private void Walk()
        {
            _moveSpeed = walkSpeed;
        }

        private void Sprint()
        {
            _moveSpeed = sprintSpeed;
        }

        private void MovePlayer()
        {
            _horizontalInput = _inputManager.horizontalInput;
            _verticalInput = _inputManager.verticalInput;
            
            _moveDirection = camOrientation.forward * _verticalInput + camOrientation.right * _horizontalInput;

            
            switch (_currentState)
            {
                case PlayerState.Idle:
                case PlayerState.Walking:
                case PlayerState.Sprinting:
                case PlayerState.Crouching:
                    if (OnSlope() && !_exitingSlope)
                        ApplyOnSlopeForce();
                    else if (_grounded)
                        ApplyOnGroundForce();
                    break;

                case PlayerState.InAir:
                    ApplyOnAirForce();
                    break;
            
                case PlayerState.Dashing:
                    Dash();
                    break;
            }
        }

        private void Dash()
        {
            if (!_canDash || _isDashing) return;

            StartCoroutine(DashCoroutine());
        }
        
        private IEnumerator DashCoroutine()
        {
            _isDashing = true;
            _canDash = false;
            
            // Determine the dash direction.
            Vector3 dashDirection;

            if (_moveDirection.magnitude > 0)
            {
                // Use the current movement direction.
                dashDirection = _moveDirection.normalized;
            }
            else
            {
                // Dash backward relative to the camera's orientation.
                dashDirection = -camOrientation.forward.normalized;
            }

            float dashEndTime = Time.time + dashDuration;

            // Apply force incrementally with deceleration.
            while (Time.time < dashEndTime)
            {
                float remainingTime = dashEndTime - Time.time;
                float speedModifier = remainingTime / dashDuration; // Linearly reduce the force.

                _rb.AddForce(dashDirection * (dashSpeed * speedModifier), ForceMode.Force);

                yield return new WaitForFixedUpdate(); // Wait for the next physics update.
            }

            _isDashing = false;

            // Start cooldown for the dash.
            yield return new WaitForSeconds(dashCooldown);
            _canDash = true;
        }

        
        private void ApplyOnGroundForce()
        {
            _rb.AddForce(_moveDirection.normalized * (_moveSpeed * 50f), ForceMode.Force);
        }

        private void ApplyOnAirForce()
        {
            _rb.AddForce(_moveDirection.normalized * (_moveSpeed * 50f * airMultiplier), ForceMode.Force);
        }

        private void ApplyOnSlopeForce()
        {
            _rb.AddForce(GetSlopeMoveDirection() * (_moveSpeed * 80f), ForceMode.Force);
            _rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }
        
        private bool OnSlope()
        {
            if (Physics.Raycast(transform.position, Vector3.down, out _slopeHit, playerHeight * 1.5f, GroundLayer))
            {
                float angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
                return angle <= maxSlopeAngle && angle > 0f;
            }
            return false;
        }
        
        private Vector3 GetSlopeMoveDirection()
        {
            return Vector3.ProjectOnPlane(_moveDirection.normalized, _slopeHit.normal).normalized;
        }

        private void SpeedControl()
        {
            if (OnSlope() && !_exitingSlope)
            {
                // Get velocity relative to the slope.
                Vector3 slopeVelocity = Vector3.ProjectOnPlane(_rb.linearVelocity, _slopeHit.normal);

                // Clamp the velocity to the desired move speed.
                if (slopeVelocity.magnitude > _moveSpeed)
                    _rb.linearVelocity = slopeVelocity.normalized * _moveSpeed;
                
            }
            else
            {
                // For flat ground or in the air, limit the horizontal velocity.
                Vector3 flatVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);

                if (flatVelocity.magnitude > _moveSpeed)
                {
                    Vector3 limitedVelocity = flatVelocity.normalized * _moveSpeed;
                    _rb.linearVelocity = new Vector3(limitedVelocity.x, _rb.linearVelocity.y, limitedVelocity.z);
                }
            }
        }
        
        private void Jump()
        {
            _exitingSlope = true;
            
            Vector3 velocity = _rb.linearVelocity;
            _rb.linearVelocity = new Vector3(velocity.x, 0f, velocity.z);
            
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            
            Invoke(nameof(ResetJump), jumpCooldown);
        }
        
        private void ResetJump()
        {
            _readyToJump = true;
            _exitingSlope = false;
        }
    }
}