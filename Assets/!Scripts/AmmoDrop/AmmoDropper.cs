using UnityEngine;

namespace _Scripts.AmmoDrop
{
    public class AmmoDropper : MonoBehaviour
    {
        [Header("Ammo Drop Settings")] [SerializeField]
        private AmmoDropData[] possibleAmmoDrops;

        [SerializeField] private float dropUpwardForce = 5f;
        [SerializeField] private float dropScatterRadius = 1f;

        public void DropAmmo()
        {
            foreach (var dropData in possibleAmmoDrops)
                if (Random.Range(0f, 100f) <= dropData.dropChance)
                    SpawnAmmoDrop(dropData);
        }

        private void SpawnAmmoDrop(AmmoDropData dropData)
        {
            // Calculate random position within scatter radius.
            var randomCircle = Random.insideUnitCircle * dropScatterRadius;
            var dropPosition = transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);

            var ammoPickup = Instantiate(
                dropData.ammoPrefab,
                dropPosition,
                Quaternion.identity);

            // Apply upward force and random rotation.
            var rb = ammoPickup.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(Vector3.up * dropUpwardForce, ForceMode.Impulse);
                rb.AddTorque(Random.insideUnitSphere * 10f, ForceMode.Impulse);
            }

            // Set random ammo amount within range.
            var pickup = ammoPickup.GetComponent<AmmoPickup>();
            if (pickup != null)
            {
                var randomAmount = Random.Range(
                    dropData.ammoAmountRange.x,
                    dropData.ammoAmountRange.y + 1
                );
                pickup.SetAmmoAmount(randomAmount);
            }
        }
    }
}