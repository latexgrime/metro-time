using System.Collections.Generic;
using UnityEngine;

namespace Metro.Animation
{
    /// <summary>
    /// Manages doors in different rooms based on enemy count and player progress.
    /// Unlocks doors when enemies are eliminated and closes doors when the player moves to the next room.
    /// </summary>
    public class MetroDoorManager : MonoBehaviour
    {
        #region Variables

        [Header("- Door Related Variables")]
        [Tooltip("List of doors in the scene. Ensure each room has its corresponding door.")]
        [SerializeField] private GameObject[] metroDoors;
        [Tooltip("Animator components for doors. Must match the metroDoors array order.")]
        [SerializeField] private Animator[] doorAnimators;

        [Header("- Enemy Tracking")]
        [Tooltip("Tracks enemies in each room dynamically.")]
        private List<GameObject>[] enemiesPerRoom;

        [Header("- Game Progress Tracking")]
        [Tooltip("Tracks the player's current room index.")]
        [SerializeField] private int currentRoomIndex = 0;

        #endregion

        #region Unity Methods

        private void Start()
        {
            InitializeEnemyLists();
        }

        private void Update()
        {
            CheckRoomProgress();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initializes the enemy lists dynamically for each room.
        /// </summary>
        private void InitializeEnemyLists()
        {
            enemiesPerRoom = new List<GameObject>[metroDoors.Length + 1]; // Extra slot for the boss room
            for (int i = 0; i < enemiesPerRoom.Length; i++)
            {
                enemiesPerRoom[i] = new List<GameObject>();
            }
        }

        /// <summary>
        /// Checks if the current room's enemies are all eliminated.
        /// </summary>
        private void CheckRoomProgress()
        {
            if (currentRoomIndex >= enemiesPerRoom.Length) return;

            // Check if all enemies in the current room are defeated
            if (enemiesPerRoom[currentRoomIndex].Count == 0)
            {
                OpenDoor(currentRoomIndex);
            }
        }

        /// <summary>
        /// Opens the door for the current room if applicable.
        /// </summary>
        private void OpenDoor(int roomIndex)
        {
            if (roomIndex < metroDoors.Length)
            {
                doorAnimators[roomIndex].SetTrigger("Open");
            }
        }

        /// <summary>
        /// Closes the door for the current room if applicable.
        /// </summary>
        private void CloseDoor(int roomIndex)
        {
            if (roomIndex < metroDoors.Length)
            {
                doorAnimators[roomIndex].SetTrigger("Close");
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds an enemy to the respective room's enemy list.
        /// Call this when an enemy spawns.
        /// </summary>
        public void RegisterEnemy(GameObject enemy, int roomIndex)
        {
            if (roomIndex < enemiesPerRoom.Length)
            {
                enemiesPerRoom[roomIndex].Add(enemy);
            }
        }

        /// <summary>
        /// Removes an enemy from the list when it dies.
        /// Call this from the enemy script.
        /// </summary>
        public void EnemyDeactivated(GameObject enemy, int roomIndex)
        {
            if (roomIndex < enemiesPerRoom.Length)
            {
                enemiesPerRoom[roomIndex].Remove(enemy);
            }
        }

        /// <summary>
        /// Call this when the player enters the next room.
        /// </summary>
        public void PlayerEnteredNextRoom()
        {
            if (currentRoomIndex < metroDoors.Length)
            {
                CloseDoor(currentRoomIndex);
            }

            currentRoomIndex++;
        }

        #endregion

    }
}
