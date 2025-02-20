using System.Collections;
using UnityEngine;

namespace MainGame.Light
{
    /// <summary>
    /// Rotates the Light up and down, the higher the numbers the more violent the shake is.
    /// </summary>
    public class LightGlitch : MonoBehaviour
    {
        #region Variables
        [SerializeField] private float glitchAmount = 5f;
        [SerializeField] private float glitchSpeed = 10f;
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
                StartCoroutine(GlitchLight());
            }
        }

        #region Private Functions
        private IEnumerator GlitchLight()
        {
            _isGlitching = true;

            float glitchDuration = 0.2f;
            float timeElapsed = 0f;

            Quaternion originalRotation = transform.rotation;

            while (timeElapsed < glitchDuration)
            {
                float glitchY = Random.Range(-glitchAmount, glitchAmount);
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, glitchY, transform.rotation.eulerAngles.z);

                timeElapsed += Time.deltaTime * glitchSpeed;
                yield return null;
            }

            float restoreTime = 0.1f;
            float restoreElapsed = 0f;

            while (restoreElapsed < restoreTime)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, originalRotation, restoreElapsed / restoreTime);
                restoreElapsed += Time.deltaTime;
                yield return null;
            }

            transform.rotation = originalRotation;
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
