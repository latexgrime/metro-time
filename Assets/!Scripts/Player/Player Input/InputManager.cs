﻿using _Scripts.Player.Movement;
using _Scripts.Status_System;
using _Scripts.Weapon_Systems;
using _Scripts.Weapon_Systems.Weapons_Logic;
using UnityEngine;

namespace _Scripts.Player
{
    public class InputManager : MonoBehaviour
    {
        private PlayerControls _playerControls;
        private WeaponHandler _weaponHandler;
        private PlayerMovement _playerMovement;
        
        public Vector2 movementInput;
        public Vector2 cameraInput;

        [Header("Camera Input")] public float cameraInputX;
        public float cameraInputY;

        [Header("Moving Input")] 
        public float moveAmount;
        public float verticalInput;
        public float horizontalInput;

        [Header("Player Actions Input")] public bool sprintInput;
        public bool walkInput;
        public bool jumpInput;
        public bool dashInput;
        public bool interactInput;
        public bool crouchInput;
        public bool zoomInput;
        public bool throwInput;

        [Header("Weapon Input")] public bool reloadInput;
        public bool aimInput;
        public bool shootInput;
        public float weaponScrollInput;

        private void Start()
        {
            _weaponHandler = GetComponent<WeaponHandler>();
            _playerMovement = GetComponent<PlayerMovement>(); 
        }
        
        private void OnEnable()
        {
            if (_playerControls == null)
            {
                _playerControls = new PlayerControls();

                // Movement
                _playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
                _playerControls.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();

                // Actions
                _playerControls.PlayerActions.Sprint.performed += i => sprintInput = true;
                _playerControls.PlayerActions.Sprint.canceled += i => sprintInput = false;

                _playerControls.PlayerActions.Jump.performed += i => jumpInput = true;
                _playerControls.PlayerActions.Jump.canceled += i => jumpInput = false;

                _playerControls.PlayerActions.Crouch.performed += i => crouchInput = true;
                _playerControls.PlayerActions.Crouch.canceled += i => crouchInput = false;

                _playerControls.PlayerActions.Dash.performed += i => dashInput = true;
                _playerControls.PlayerActions.Dash.canceled += i => dashInput = false;

                _playerControls.PlayerActions.Interact.performed += i => interactInput = true;
                _playerControls.PlayerActions.Interact.canceled += i => interactInput = false;

                _playerControls.PlayerActions.ThrowObject.performed += i => throwInput = true;
                _playerControls.PlayerActions.ThrowObject.canceled += i => throwInput = false;

                _playerControls.PlayerActions.ZoomCamera.performed += i => zoomInput = true;
                _playerControls.PlayerActions.ZoomCamera.canceled += i => zoomInput = false;

                _playerControls.PlayerActions.Reload.performed += i => reloadInput = true;
                _playerControls.PlayerActions.Reload.canceled += i => reloadInput = false;

                _playerControls.PlayerActions.Aim.performed += i => aimInput = true;
                _playerControls.PlayerActions.Aim.canceled += i => aimInput = false;

                _playerControls.PlayerActions.Shoot.performed += i => shootInput = true;
                _playerControls.PlayerActions.Shoot.canceled += i => shootInput = false;

                _playerControls.PlayerActions.WeaponScroll.performed += i => weaponScrollInput = i.ReadValue<float>();
                _playerControls.PlayerActions.WeaponScroll.canceled += i => weaponScrollInput = 0f;
            }

            _playerControls.Enable();
        }

        private void OnDisable()
        {
            _playerControls.Disable();
        }

        private void Update()
        {
            UpdateMovementInput();
            UpdateCameraInput();
            
            UpdateWeaponAnimationState();
        }

        private void UpdateWeaponAnimationState()
        {
            // Check if movement is disabled or player is stunned/slowed
            if (!_playerMovement._movementEnabled)
            {
                if (_weaponHandler != null)
                {
                    Weapon currentWeapon = _weaponHandler.GetCurrentWeapon();
                    if (currentWeapon != null)
                    {
                        currentWeapon.UpdateMovementState(false, 0f);
                    }
                }
                return;
            }
    
            // Calculate movement speed.
            Vector3 movement = new Vector3(horizontalInput, 0, verticalInput);
            float movementSpeed = movement.magnitude;

            // Check if sprinting and grounded.
            bool isSprinting = Input.GetKey(KeyCode.LeftShift) && 
                               movementSpeed > 0 && 
                               _playerMovement.CurrentState != PlayerState.InAir && 
                               _playerMovement.CurrentState != PlayerState.Jumping;

            // Prevent sprinting if player is stunned or slowed
            StatusEffectHandler statusEffectHandler = GetComponent<StatusEffectHandler>();
            if (statusEffectHandler != null && 
                (statusEffectHandler.IsSlowed || statusEffectHandler.IsStunned))
            {
                isSprinting = false;
                movementSpeed = Mathf.Min(movementSpeed, 0.5f);
            }

            // If we're in the air or jumping, force movementSpeed to walking speed.
            if (_playerMovement.CurrentState == PlayerState.InAir || 
                _playerMovement.CurrentState == PlayerState.Jumping)
            {
                // Normalize movement speed to walking when in air.
                movementSpeed = Mathf.Min(movementSpeed, 0.5f);
                isSprinting = false;
            }

            // Update weapon animation state.
            if (_weaponHandler != null)
            {
                Weapon currentWeapon = _weaponHandler.GetCurrentWeapon();
                if (currentWeapon != null)
                {
                    currentWeapon.UpdateMovementState(isSprinting, movementSpeed);
                }
            }
        }

        private void UpdateMovementInput()
        {
            verticalInput = movementInput.y;
            horizontalInput = movementInput.x;
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));
        }

        private void UpdateCameraInput()
        {
            cameraInputX = cameraInput.x;
            cameraInputY = cameraInput.y;
        }
    }
}