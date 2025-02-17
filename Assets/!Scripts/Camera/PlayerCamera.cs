using System;
using _Scripts.Player;
using NALEO._Scripts;
using UnityEngine;

namespace _Scripts.Camera
{
    /// <summary>
    ///     This script is in charge on handling the Input for the camera and handling the camera itself.
    /// </summary>
    public class PlayerCamera : MonoBehaviour
    {
        private InputManager _inputManager;
        
        private float _xDefaultSensitivity;
        private float _yDefaultSensitivity;
        
        [Range(0,1)]
        [SerializeField] private float sensitivityX;
        [Range(0,1)]
        [SerializeField] private float sensitivityY;

        private Transform _camTargetOrientation;

        private float _xRotation;
        private float _yRotation;

        private void Start()
        {
            _inputManager = FindFirstObjectByType<InputManager>();
            
            // Makes cursor invisible and locks in in the middle of the screen.
            MakeCursorInvisible();

            _xDefaultSensitivity = sensitivityX;
            _yDefaultSensitivity = sensitivityY;
            
            // Get the reference to the CamTargetOrientation game object.
            _camTargetOrientation = GameObject.FindGameObjectWithTag("Player").transform.Find("CamTargetOrientation");
        }

        public void MakeCursorInvisible()
        {
            // This locks the cursor in the middle of the screen.
            Cursor.lockState = CursorLockMode.Locked;
            // Make the cursor invisible.
            Cursor.visible = false;
        }

        // In case its used in Unity Events.
        public void MakeCursorVisible()
        {
            // This unlocks the cursor.
            Cursor.lockState = CursorLockMode.None;
            // Make the cursor visible.
            Cursor.visible = true;
        }

        public void SetDefaultSensitivityValues()
        {
            sensitivityX = _xDefaultSensitivity;
            sensitivityY = _yDefaultSensitivity;
        }

        public Vector2 GetSensitivityValues()
        {
            return new Vector2(sensitivityX,sensitivityY);
        }
        
        public void SetXSensitivityValue(float value)
        {
            sensitivityX = value;
        }

        public void SetYSensitivityValue(float value)
        {
            sensitivityY = value;
        }

        public void SetBothSensitivityValues(float valueX, float valueY)
        {
            sensitivityX = valueX;
            sensitivityY = valueY;
        }

        private void Update()
        {
            if (Time.timeScale == 0f) return; // Prevents camera movement when paused.

            // Handle player input.
            var mouseX = _inputManager.cameraInputX;
            var mouseY = _inputManager.cameraInputY;

            // Handle camera rotation.
            _yRotation += mouseX * sensitivityX;
            _xRotation -= mouseY * sensitivityY;

            // Clamp rotation so player can't spin around like crazy.
            _xRotation = Math.Clamp(_xRotation, -90f, 90f);

            // Rotate camera and the cameraOrientation object.
            transform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0);
            _camTargetOrientation.rotation = Quaternion.Euler(0, _yRotation, 0);
        }

    }
}