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
            if (other.CompareTag( "PlayerCollider"))
            {
                Debug.Log("Change sprites");
                if (spriteRenderers.Length == 0 || spritesToRender.Length == 0)
                    return;

                for (int i = 0; i < spriteRenderers.Length; i++)
                {
                    if (i < spritesToRender.Length)
                    {
                        spriteRenderers[i].sprite = spritesToRender[i];
                    }
                }

                gameObject.SetActive(false);

            }
        }
    }
}