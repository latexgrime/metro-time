using UnityEngine;

namespace _Scripts.Camera
{
    /// <summary>
    ///     This scripts just sets the position of the camera holder to the cameraTargetPosition.
    /// </summary>
    public class MoveCameraToPlayerPosition : MonoBehaviour
    {
        private Transform _cameraTargetPosition;

        private void Start()
        {
            // Get the reference to the CamTargetPosition game object.
            _cameraTargetPosition = GameObject.FindGameObjectWithTag("Player").transform.Find("CamTargetPosition");
        }

        private void Update()
        {
            transform.position = _cameraTargetPosition.position;
        }
    }
}