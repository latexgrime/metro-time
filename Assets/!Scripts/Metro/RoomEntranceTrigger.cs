using UnityEngine;
using Metro.Animation;

namespace Metro.Room
{

    public class RoomEntranceTrigger : MonoBehaviour
    {
        #region Variables
        [Tooltip("Reference MetroDoorManager script into this slot.")]
        [SerializeField] private MetroDoorManager doorManager;

        [Tooltip("Index of the room this trigger belongs to.")]
        [SerializeField] private int roomIndex;
        #endregion

        public void OnTriggerEnter(Collider other)
        {
            if (doorManager != null)
            {
                doorManager.PlayerEnteredNextRoom();
            }

            gameObject.SetActive(false);
        }
    }
}