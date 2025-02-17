using UnityEngine;

namespace _Scripts.AmmoDrop
{
    public class MinMaxAttribute : PropertyAttribute
    {
        public float min;
        public float max;
        
        public MinMaxAttribute(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }
}