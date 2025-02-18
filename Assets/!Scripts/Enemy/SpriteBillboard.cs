using UnityEngine;

namespace _Scripts.Enemy
{
    public class SpriteBillboard : MonoBehaviour
    {
        [Header("- Billboard Settings")]
        [Tooltip("When true, sprite only rotates on Y axis. When false, sprite fully faces camera.")]
        [SerializeField] private bool lockYAxis = true;
        
        private UnityEngine.Camera _mainCamera;

        private void Start()
        {
            _mainCamera = UnityEngine.Camera.main;
        }

        private void LateUpdate()
        {
            if (_mainCamera == null) return;

            if (lockYAxis)
            {
                // Keep the sprite vertical but rotating to face camera.
                Vector3 targetPosition = _mainCamera.transform.position;
                targetPosition.y = transform.position.y;
                transform.LookAt(targetPosition);
            }
            else
            {
                // Full billboard - always face camera completely.
                transform.LookAt(transform.position + _mainCamera.transform.rotation * Vector3.forward,
                    _mainCamera.transform.rotation * Vector3.up);
            }
        }
    }
}