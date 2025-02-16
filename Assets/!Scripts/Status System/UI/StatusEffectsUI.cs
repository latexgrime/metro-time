using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Status_System.UI
{
    public class StatusEffectUI : MonoBehaviour
    {
        [Header("- Status Bar References")]
        [SerializeField] private Slider stunBar;
        [SerializeField] private Slider slowBar;

        [Header("- Icon References")]
        [SerializeField] private Image stunIcon;
        [SerializeField] private Image slowIcon;
        
        [Header("- Colors")]
        [SerializeField] private Color stunBarColor = new Color(1f, 0.8f, 0f, 1f);
        [SerializeField] private Color slowBarColor = new Color(0f, 0.7f, 1f, 1f);
        
        [Header("- Animation")]
        [SerializeField] private float pulseSpeed = 2f;
        [SerializeField] private float pulseMinAlpha = 0.5f;
        [SerializeField] private float pulseMaxAlpha = 1f;

        private void Start()
        {
            // Set up initial colors through code in case anyone forgets to do it in the scene view/inspector.
            if (stunBar != null)
            {
                stunBar.fillRect.GetComponent<Image>().color = stunBarColor;
            }
            
            if (slowBar != null)
            {
                slowBar.fillRect.GetComponent<Image>().color = slowBarColor;
            }

            // Initially hide icons.
            if (stunIcon != null) stunIcon.enabled = false;
            if (slowIcon != null) slowIcon.enabled = false;
        }

        private void Update()
        {
            // Pulse active effect icons.
            float pulse = Mathf.Lerp(pulseMinAlpha, pulseMaxAlpha, 
                (Mathf.Sin(Time.time * pulseSpeed) + 1) * 0.5f);

            if (stunIcon != null && stunIcon.enabled)
            {
                Color c = stunIcon.color;
                c.a = pulse;
                stunIcon.color = c;
            }

            if (slowIcon != null && slowIcon.enabled)
            {
                Color c = slowIcon.color;
                c.a = pulse;
                slowIcon.color = c;
            }
        }

        // Methods in case we need them in the future.
        public void UpdateStunBar(float value)
        {
            if (stunBar != null)
                stunBar.value = value;
        }

        public void UpdateSlowBar(float value)
        {
            if (slowBar != null)
                slowBar.value = value;
        }

        public void SetStunIconActive(bool active)
        {
            if (stunIcon != null)
                stunIcon.enabled = active;
        }

        public void SetSlowIconActive(bool active)
        {
            if (slowIcon != null)
                slowIcon.enabled = active;
        }
    }
}