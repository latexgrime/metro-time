using System.Collections;
using UnityEngine;

namespace Mainmenu.Camera
{
    /// <summary>
    /// Moves the camera left and right, the higher the numbers the more violent the shake is.
    /// </summary>

    public class CameraGlitch
        : MonoBehaviour
    {
        #region Variables
        [SerializeField] private float glitchAmount = 0.05f;
        [SerializeField] private float glitchSpeed = 1f;
        [SerializeField] private float glitchIntervalMinimum = 3f;
        [SerializeField] private float glitchIntervalMaximum = 8f;
        private float _nextGlitchTime;
        private bool _isGlitching;
        #endregion

        private void Start()
        {
            SetNextGlitchTime();
        }

        private void Update()
        {
            if (Time.time >= _nextGlitchTime && !_isGlitching)
            {
                StartCoroutine(GlitchTrain());
            }
        }

        #region Private Functions
        private IEnumerator GlitchTrain()
        {
            _isGlitching = true;

            float shakeDuration = 0.2f;
            float timeElapsed = 0f;

            Vector3 originalPosition = transform.position;


            while (timeElapsed < shakeDuration)
            {
                float glitchY = Random.Range(-glitchAmount, glitchAmount);
                transform.position += new Vector3(0,glitchY, 0);

                timeElapsed += Time.deltaTime;
                yield return null;
            }

            transform.position = originalPosition;
            _isGlitching = false;
            SetNextGlitchTime();
        }

        private void SetNextGlitchTime()
        {
            _nextGlitchTime = Time.time + Random.Range(glitchIntervalMinimum, glitchIntervalMaximum);
        }
        #endregion

    }
}