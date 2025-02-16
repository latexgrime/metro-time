using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Status_System.UI
{
    public class StatusEffectUI : MonoBehaviour
    {
        [Header("- Effect Group References")]
        [SerializeField] private GameObject stunEffectGroup;
        [SerializeField] private GameObject freezeEffectGroup;

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
            if (stunBar != null)
            {
                stunBar.fillRect.GetComponent<Image>().color = stunBarColor;
            }
            
            if (slowBar != null)
            {
                slowBar.fillRect.GetComponent<Image>().color = slowBarColor;
            }

            SetStunGroupActive(false);
            SetFreezeGroupActive(false);
            if (stunIcon != null) stunIcon.enabled = false;
            if (slowIcon != null) slowIcon.enabled = false;
        }

        private void Update()
        {
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

        public void UpdateStunBar(float value)
        {
            if (stunBar != null)
            {
                stunBar.value = value;
                SetStunGroupActive(value > 0);
            }
        }

        public void UpdateSlowBar(float value)
        {
            if (slowBar != null)
            {
                slowBar.value = value;
                SetFreezeGroupActive(value > 0);
            }
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

        private void SetStunGroupActive(bool active)
        {
            if (stunEffectGroup != null)
                stunEffectGroup.SetActive(active);
        }

        private void SetFreezeGroupActive(bool active)
        {
            if (freezeEffectGroup != null)
                freezeEffectGroup.SetActive(active);
        }
    }
}