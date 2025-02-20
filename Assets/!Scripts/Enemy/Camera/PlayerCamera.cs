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
        
        private float _xDefaultSensitivity = 0.5f;
        private float _yDefaultSensitivity = 0.5f;
        
        [Range(0,1)]
        [SerializeField] private float sensitivityX = 0.5f;
        [Range(0,1)]
        [SerializeField] private float sensitivityY = 0.5f;

        private Transform _camTargetOrientation;

        private float _xRotation;
        private float _yRotation;

        private void Start()
        {
            _inputManager = FindFirstObjectByType<InputManager>();
            
            // Set inspector values as defaults if they are different from the initialized values
            if (sensitivityX != _xDefaultSensitivity) _xDefaultSensitivity = sensitivityX;
            if (sensitivityY != _yDefaultSensitivity) _yDefaultSensitivity = sensitivityY;
            
            // Load saved sensitivity values from PlayerPrefs if they exist
            LoadSavedSensitivity();
            
            // Makes cursor invisible and locks in in the middle of the screen.
            MakeCursorInvisible();
            
            // Get the reference to the CamTargetOrientation game object.
            _camTargetOrientation = GameObject.FindGameObjectWithTag("Player").transform.Find("CamTargetOrientation");
        }

        private void LoadSavedSensitivity()
        {
            // -1 is used as a flag to indicate no saved value
            float savedX = PlayerPrefs.GetFloat("Camera_Sensitivity_X", -1f);
            float savedY = PlayerPrefs.GetFloat("Camera_Sensitivity_Y", -1f);
            
            // Only apply saved values if they exist
            if (savedX >= 0f) sensitivityX = savedX;
            if (savedY >= 0f) sensitivityY = savedY;
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
            return new Vector2(sensitivityX, sensitivityY);
        }
        
        public void SetXSensitivityValue(float value)
        {
            sensitivityX = Mathf.Clamp01(value);
        }

        public void SetYSensitivityValue(float value)
        {
            sensitivityY = Mathf.Clamp01(value);
        }

        public void SetBothSensitivityValues(float valueX, float valueY)
        {
            sensitivityX = Mathf.Clamp01(valueX);
            sensitivityY = Mathf.Clamp01(valueY);
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