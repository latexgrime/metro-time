using UnityEngine;

namespace Metro.Scroller
{
    public class MetroBackgroundObjectsManager : MonoBehaviour
    {
        #region Variables
        [SerializeField] private SpriteRenderer[] spriteRenderers;
        [SerializeField] private Sprite[] spritesToRender;
        #endregion

        private void OnTriggerEnter(Collider other)
        {
            // Ensure sprite arrays are not empty
            if (spriteRenderers.Length == 0 || spritesToRender.Length == 0)
                return;

            // Assign random sprites from the array to each renderer
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                if (i < spritesToRender.Length) // Prevent out-of-bounds errors
                {
                    spriteRenderers[i].sprite = spritesToRender[i];
                }
            }

            // Disable the object after processing
            gameObject.SetActive(false);
        }
    }
}
