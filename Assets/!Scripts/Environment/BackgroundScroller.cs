using System.Collections.Generic;
using UnityEngine;

namespace BackgroundScroller
{


    public class BackgroundScroller : MonoBehaviour
    {
        #region Variables
        [SerializeField] private float spriteScrollSpeed = 0.1f;
        [SerializeField] private Material scrollMaterial;
        #endregion

        private void Update()
        {
            //Debug.Log("Update running: " + Time.time);
            float offsetX = Time.time * spriteScrollSpeed;
            scrollMaterial.SetVector("_Offset", new Vector4(offsetX, 0, 0, 0));
        }

    }
}