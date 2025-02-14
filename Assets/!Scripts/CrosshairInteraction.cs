using UnityEngine;
using UnityEngine.UI;

namespace _Scripts
{
    public class CrosshairInteraction : MonoBehaviour
    {
        private Animator crosshairAnimator;
        private Image crosshairImage;

        [Header("- Crosshair settings")] 
        [SerializeField] private GameObject crosshair;
        [SerializeField] private Color idleColor;
        [SerializeField] private Color interactionColor;
        [SerializeField] private float objectInteractionDistance = 3f;
    
        private void Start()
        {
            GetComponents();
        }

        private void GetComponents()
        {
            crosshairAnimator = crosshair.GetComponent<Animator>();
            crosshairImage = crosshair.GetComponent<Image>();
            crosshairImage.color = idleColor;
        }

        private void Update()
        {
            CrosshairInteractionCheck();
        }

        // Cast a ray to check if there is an interactable object in front.
        private void CrosshairInteractionCheck()
        {
            if (!Physics.Raycast(transform.position, transform.forward, out var hit, objectInteractionDistance))
            {
                crosshairAnimator.SetBool("Interacting", false);
                crosshairImage.color = idleColor;
                return;
            }
            CrosshairInteractionAnimation(hit);
        }
    
        // Updates the crosshair to tell the player whatever they're looking at is interactable.
        private void CrosshairInteractionAnimation(RaycastHit hit)
        {
            // Set the crosshair animation if the player is looking at an object that can be picked up and set the crosshair blue.
            if (hit.transform.CompareTag("CanPickUp"))
            {
                crosshairAnimator.SetBool("Interacting", true);
                crosshairImage.color = interactionColor;
            }
        }
    }
}
