using UnityEngine;
// using UnityEngine.Rendering.PostProcessing;

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
        [SerializeField] private float leftOutBound = -20f;
        [Tooltip("Add your leftOutBound with its positive form to set your far right position consistently. i.e. -20f -> 40f. (-20 + 20 + 20. (20 + 20 is 40))")]
        [SerializeField] private float farRightPosition = 40f;
        #endregion

        void Update()
        {
            for (int i = 0; i < layers.Length; i++)
            {
                layers[i].position += Vector3.left * baseSpeed * speedMultipliers[i] * Time.deltaTime;

                if (layers[i].position.x < leftOutBound)
                {
                    layers[i].position += new Vector3(farRightPosition, 0, 0);
                }
            }
        }
    }
}