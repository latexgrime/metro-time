using UnityEngine;

namespace Mainmenu.ImageScroll
{
    /// <summary>
    /// 
    /// </summary>

    public class LayerScroll : MonoBehaviour
    {
        #region Variables
        [SerializeField] private Transform[] layers;
        [SerializeField] private float[] speedMultipliers;
        [SerializeField] private float baseSpeed = 2f;
        #endregion

        void Update()
        {
            for (int i = 0; i < layers.Length; i++)
            {
                layers[i].position += Vector3.left * baseSpeed * speedMultipliers[i] * Time.deltaTime;

                if (layers[i].position.x < -10f)
                {
                    layers[i].position += new Vector3(20f, 0, 0);
                }
            }
        }
    }
}