using UnityEngine;

namespace NALEO._Scripts.Camera.Utilities
{
    public class CameraZoom : MonoBehaviour
    {
        private InputManager _inputManager;
        private PlayerCamera _playerCamera;
        private UnityEngine.Camera _camera;

        private float _defaultFOV;
        private float _targetFOV;

        private bool _zoomToggle;

        [SerializeField] private float zoomFOV = 30f;
        [SerializeField] private float zoomDampSpeed = 5f;
        [SerializeField] private float zoomDampTime = 0.2f;
        [SerializeField] private float decreaseSensitivityFactor;

        private void Start()
        {
            InitializeValues();
        }

        private void InitializeValues()
        {
            _inputManager = FindFirstObjectByType<InputManager>();
            _camera = GetComponent<UnityEngine.Camera>();
            _playerCamera = GetComponent<PlayerCamera>();
            _defaultFOV = _camera.fieldOfView;
            _targetFOV = _defaultFOV;
            decreaseSensitivityFactor = zoomFOV / _defaultFOV;
        }

        private void Update()
        {
            CheckZoomInput();
            SetFieldOfViewValue();
        }

        private void SetFieldOfViewValue()
        {
            _camera.fieldOfView = Mathf.SmoothDamp(_camera.fieldOfView, _targetFOV, ref zoomDampSpeed, zoomDampTime);
        }

        private void CheckZoomInput()
        {
            if (_inputManager.zoomInput && !_zoomToggle)
            {
                ZoomCamera();
            }
            else if (!_inputManager.zoomInput && _zoomToggle)
            {
                UnzoomCamera();
            }
        }

        private void ZoomCamera()
        {
            _playerCamera.SetBothSensitivityValues(
                    _playerCamera.GetSensitivityValues().x * decreaseSensitivityFactor,
                    _playerCamera.GetSensitivityValues().y * decreaseSensitivityFactor);
            _targetFOV = zoomFOV;
            _zoomToggle = true;
        }

        private void UnzoomCamera()
        {
            _playerCamera.SetDefaultSensitivityValues();
            _targetFOV = _defaultFOV;
            _zoomToggle = false;
        }
    }
}