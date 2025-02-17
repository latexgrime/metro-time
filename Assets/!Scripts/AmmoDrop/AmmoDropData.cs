using UnityEngine;

namespace _Scripts.AmmoDrop
{
    [System.Serializable]
    public class AmmoDropData
    {
        public GameObject ammoPrefab;
        [Range(0, 100)] public float dropChance = 50f;
        [MinMax(1, 50)] public Vector2Int ammoAmountRange = new Vector2Int(10, 30);
    }
}