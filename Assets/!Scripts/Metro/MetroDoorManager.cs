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

        [Header("- Enemy Detection Settings")]
        [Tooltip("Tag used to identify enemies.")]
        [SerializeField] private string enemyTag = "Enemy";

        #endregion

        #region Unity Methods

        private void Start()
        {
            InitializeEnemyLists();
            AutoRegisterExistingEnemies();
        }

        private void Update()
        {
            CheckRoomProgress();
        }

        #endregion

        #region Private Methods

        private void InitializeEnemyLists()
        {
            enemiesPerRoom = new List<GameObject>[metroDoors.Length + 1];
            for (int i = 0; i < enemiesPerRoom.Length; i++)
            {
                enemiesPerRoom[i] = new List<GameObject>();
            }
        }

        /// <summary>
        /// Automatically registers enemies at the start of the game.
        /// </summary>
        private void AutoRegisterExistingEnemies()
        {
            GameObject[] allEnemies = GameObject.FindGameObjectsWithTag(enemyTag);

            foreach (GameObject enemy in allEnemies)
            {
                Metro.Enemy.MetroEnemy roomAssignment = enemy.GetComponent<Metro.Enemy.MetroEnemy>();

                if (roomAssignment != null)
                {
                    int roomIndex = roomAssignment.roomIndex;

                    if (roomIndex >= 0 && roomIndex < enemiesPerRoom.Length)
                    {
                        RegisterEnemy(enemy, roomIndex);
                    }
                    else
                    {
                        Debug.LogWarning($"Enemy '{enemy.name}' has an invalid room index ({roomIndex}). Skipping registration.");
                    }
                }
                else
                {
                    Debug.LogWarning($"Enemy '{enemy.name}' is missing the EnemyRoomAssignment script.");
                }
            }
        }

        private void CheckRoomProgress()
        {
            if (currentRoomIndex >= enemiesPerRoom.Length) return;

            if (enemiesPerRoom[currentRoomIndex].Count == 0)
            {
                OpenDoor(currentRoomIndex);
            }
        }

        private void OpenDoor(int roomIndex)
        {
            if (roomIndex < metroDoors.Length)
            {
                doorAnimators[roomIndex].SetBool("doorCanOpen", true);
            }
        }

        private void CloseDoor(int roomIndex)
        {
            if (roomIndex < metroDoors.Length)
            {
                doorAnimators[roomIndex].SetBool("doorCanOpen", false);
            }
        }

        #endregion

        #region Public Methods

        public void RegisterEnemy(GameObject enemy, int roomIndex)
        {
            if (roomIndex < enemiesPerRoom.Length)
            {
                enemiesPerRoom[roomIndex].Add(enemy);
            }
        }

        public void EnemyDeactivated(GameObject enemy, int roomIndex)
        {
            if (roomIndex < enemiesPerRoom.Length)
            {
                enemiesPerRoom[roomIndex].Remove(enemy);
            }
        }

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
