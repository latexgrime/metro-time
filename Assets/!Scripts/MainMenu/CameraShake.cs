using UnityEngine;

namespace Mainmenu.Camera
{
    /// <summary>
    /// Moves the camera left and right, the higher the numbers the more violent the shake is.
    /// </summary>

    public class CameraShake : MonoBehaviour
    {
        #region Variables
        [SerializeField] private float shakeAmount = 0.05f;
        [SerializeField] private float shakeSpeed = 1f;
        #endregion

        private void Update()
        {
            float shakeX = Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;
            transform.position += new Vector3(shakeX, 0, 0);
        }

    }
}