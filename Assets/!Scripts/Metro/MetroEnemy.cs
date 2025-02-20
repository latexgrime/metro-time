using _Scripts.Enemy;
using UnityEngine;

namespace Metro.Enemy
{
    /// <summary>
    /// Represents an enemy that is tracked by MetroDoorManager.
    /// </summary>
    public class MetroEnemy : MonoBehaviour
    {
        #region Variables
        public int roomIndex;

        [SerializeField] private BaseEnemy _baseEnemyScript;
        [SerializeField] private Metro.Animation.MetroDoorManager _doorManagerScript;
        private int _roomIndex;
        
        private bool isDeactivationReported = false;
        #endregion

        private void Start()
        {
            if (_doorManagerScript == null)
            {
                _doorManagerScript = FindObjectOfType<Metro.Animation.MetroDoorManager>();
            }
            
            // Synchronize the private variable with the public one
            _roomIndex = roomIndex;
        }

        private void Update()
        {
            // If we've already reported deactivation, don't check again
            if (isDeactivationReported) return;
            
            BaseEnemy enemy = GetComponent<BaseEnemy>();

            if (enemy != null && enemy.IsDeactivated())
            {
                _doorManagerScript.EnemyDeactivated(gameObject, _roomIndex);
                isDeactivationReported = true;
            }
        }

        public void Initialize(Metro.Animation.MetroDoorManager manager, int assignedRoomIndex)
        {
            _doorManagerScript = manager;
            _roomIndex = assignedRoomIndex;
            _doorManagerScript.RegisterEnemy(gameObject, _roomIndex);
        }
    }
}