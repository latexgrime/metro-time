using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Boss
{
    public class BossHealthUI : MonoBehaviour
    {
        [SerializeField] public Slider healthBar;
        private Boss boss;
        private void Start()
        {
            boss = FindObjectOfType<Boss>();
            if (boss != null)
            {
                healthBar.maxValue = boss.GetMaxShield();
            }
        }

        private void Update()
        {
            if (boss != null) 
            {
                healthBar.value = boss.GetCurrentShield();
            }
        }
    }
}