using UnityEngine;

namespace _Scripts.AmmoDrop
{
    public class AmmoDropper : MonoBehaviour
    {
        [Header("Ammo Drop Settings")] 
        [SerializeField] private AmmoDropData[] possibleAmmoDrops;
        [SerializeField] private float dropUpwardForce = 5f;
        [SerializeField] private float dropScatterRadius = 1f;

        public void DropAmmo()
        {
            foreach (var dropData in possibleAmmoDrops)
            {
                if (Random.Range(0f, 100f) <= dropData.dropChance)
                {
                    SpawnAmmoDrop(dropData);
                }
            }
        }

        private void SpawnAmmoDrop(AmmoDropData dropData)
        {
            float spawnHeightOffset = 1.5f;
            
            // Calculate random position within scatter radius.
            Vector2 randomCircle = Random.insideUnitCircle * dropScatterRadius;
            Vector3 dropPosition = transform.position + 
                                   new Vector3(randomCircle.x, spawnHeightOffset, randomCircle.y);
            
            GameObject ammoPickup = Instantiate(
                dropData.ammoPrefab, 
                dropPosition, 
                Quaternion.identity);

            // Calculate initial velocity with random direction.
            Vector3 randomDir = new Vector3(
                Random.Range(-1f, 1f),
                0,
                Random.Range(-1f, 1f)
            ).normalized;

            Vector3 initialVelocity = Vector3.up * dropUpwardForce + randomDir * 3f;
            
            // Initialize the custom physics.
            AmmoPickup pickup = ammoPickup.GetComponent<AmmoPickup>();
            if (pickup != null)
            {
                pickup.Initialize(initialVelocity);
                
                int randomAmount = Random.Range(
                    dropData.ammoAmountRange.x,
                    dropData.ammoAmountRange.y + 1
                );
                pickup.SetAmmoAmount(randomAmount);
            }
        }
    }
}