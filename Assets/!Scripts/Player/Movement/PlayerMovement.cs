using System.Collections;
using UnityEngine;

namespace _Scripts.Player.Movement
{
    /// <summary>
    ///     Script in charge of handling the input for the movement of the player.
    /// </summary>
    public class PlayerMovement : MonoBehaviour
    {
        private PlayerAudio _playerAudio;
        private InputManager _inputManager;
        private AudioSource _audioSource;
        private Animator _animator;
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
        [SerializeField] private float crouchSpeed = 3.5f;
        [SerializeField] private float crouchYScale = 0.5f;
        private float _startYScale;
        private bool _isCrouching = false;
        
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
        private bool _isShootingBlocked = false;
        public bool _movementEnabled = true;
        private float _moveSpeedMultiplier = 1f;

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
            _animator = GetComponentInChildren<Animator>();
            
            _rb.freezeRotation = true;
            _readyToJump = true;
            _startYScale = transform.localScale.y;
            _moveSpeed = walkSpeed;
        }

        private void Update()
        {
            GroundCheck();
            HandleStateTransitions();
            UpdateAnimations();
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
            // If movement is disabled, force idle state and skip other transitions [for the running animation happening when the player is stunned].
            if (!_movementEnabled)
            {
                TransitionToState(PlayerState.Idle);
                return;
            }

            _previousState = _currentState;
    
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
                    else if (_inputManager.sprintInput && !_isShootingBlocked && 
                             (_inputManager.horizontalInput > 0 || _inputManager.verticalInput > 0))
                        TransitionToState(PlayerState.Sprinting);
                    else if (_inputManager.crouchInput)
                        TransitionToState(PlayerState.Crouching);
                    break;
                
                case PlayerState.Walking:
                    if (!_grounded)
                        TransitionToState(PlayerState.InAir);
                    else if (_inputManager.sprintInput && !_isShootingBlocked && 
                             (_inputManager.horizontalInput > 0 || _inputManager.verticalInput > 0))
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
                
                    else if (!_inputManager.crouchInput)
                    {
                        // Check if there's enough space to stand up.
                        if (!Physics.Raycast(transform.position, Vector3.up, 
                                playerHeight * 1.1f, GroundLayer))
                        {
                            if (_inputManager.sprintInput)
                                TransitionToState(PlayerState.Sprinting);
                            else if (_inputManager.horizontalInput != 0 || _inputManager.verticalInput != 0)
                                TransitionToState(PlayerState.Walking);
                            else
                                TransitionToState(PlayerState.Idle);
                        }
                    }
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
            
            if (!_inputManager.shootInput && _isShootingBlocked)
            {
                _isShootingBlocked = false;
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
                    ExitCrouch();
                    break;
            
                case PlayerState.Jumping:
                    _exitingSlope = false;
                    break;
            
                case PlayerState.Dashing:
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
            if (_isCrouching) return; // [Crouch fix] Prevent multiple crouch attempts.
        
            _isCrouching = true;
            _moveSpeed = crouchSpeed;
        
            // Store current scale.
            Vector3 newScale = transform.localScale;
            newScale.y = _startYScale * crouchYScale;
            transform.localScale = newScale;
        
            // Add downward force to quickly enter crouch.
            _rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        private void ExitCrouch()
        {
            if (!_isCrouching) return; // Prevent multiple uncrouch attempts.
    
            _isCrouching = false;
    
            // Calculate the height difference before changing scale.
            float heightDifference = (_startYScale - transform.localScale.y) * playerHeight;
    
            // Restore original scale
            Vector3 newScale = transform.localScale;
            newScale.y = _startYScale;
            transform.localScale = newScale;
    
            // Teleport the player up by the height difference.
            transform.position += Vector3.up * heightDifference;
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
            if (!_movementEnabled) 
            {
                _rb.linearVelocity = Vector3.zero;
                _horizontalInput = 0;
                _verticalInput = 0;
                return;
            }

            _horizontalInput = _inputManager.horizontalInput;
            _verticalInput = _inputManager.verticalInput;
        
            _moveDirection = camOrientation.forward * _verticalInput + camOrientation.right * _horizontalInput;

            float effectiveSpeed = _moveSpeed * _moveSpeedMultiplier;
        
            switch (_currentState)
            {
                case PlayerState.Idle:
                case PlayerState.Walking:
                case PlayerState.Sprinting:
                case PlayerState.Crouching:
                    if (OnSlope() && !_exitingSlope)
                        _rb.AddForce(GetSlopeMoveDirection() * (effectiveSpeed * 80f), ForceMode.Force);
                    else if (_grounded)
                        _rb.AddForce(_moveDirection.normalized * (effectiveSpeed * 50f), ForceMode.Force);
                    break;

                case PlayerState.InAir:
                    _rb.AddForce(_moveDirection.normalized * (effectiveSpeed * 50f * airMultiplier), ForceMode.Force);
                    break;
        
                case PlayerState.Dashing:
                    if (_movementEnabled) Dash();
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
        
        public void ForceWalkState()
        {
            if (_currentState == PlayerState.Sprinting)
            {
                TransitionToState(PlayerState.Walking);
                _isShootingBlocked = true;
            }
        }
        
        public void SetMovementEnabled(bool enabled)
        {
            _movementEnabled = enabled;
        }

        public void SetMovementSpeedMultiplier(float multiplier)
        {
            _moveSpeedMultiplier = Mathf.Clamp(multiplier, 0f, 1f);
        }
        
        private void UpdateAnimations()
        {
            if (_animator == null) return;

            if (_movementEnabled)
            {
                // Calculate total movement magnitude.
                float horizontalMovement = Mathf.Abs(_horizontalInput);
                float verticalMovement = Mathf.Abs(_verticalInput);
                float totalMovement = Mathf.Clamp01(horizontalMovement + verticalMovement);

                // Update animation states.
                _animator.SetBool("isRunning", _currentState == PlayerState.Sprinting);
                _animator.SetBool("isWalking", _currentState == PlayerState.Walking);
                _animator.SetFloat("MovementSpeed", totalMovement);
                _animator.SetBool("isJumping", _currentState == PlayerState.Jumping);
                _animator.SetBool("isInAir", _currentState == PlayerState.InAir);
            }
            else
            {
                // Force all movement animations to stop when movement is disabled.
                _animator.SetBool("isRunning", false);
                _animator.SetBool("isWalking", false);
                _animator.SetFloat("MovementSpeed", 0f);
        
                // Keep jump/air states as they are.
                _animator.SetBool("isJumping", _currentState == PlayerState.Jumping);
                _animator.SetBool("isInAir", _currentState == PlayerState.InAir);
            }
        }
        
    }
}