using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

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

        private HashSet<GameObject> registeredEnemies = new HashSet<GameObject>();
        private HashSet<int> openedDoors = new HashSet<int>();
        private HashSet<GameObject> deactivatedEnemies = new HashSet<GameObject>();

        #endregion

        #region Unity Methods

        private void Start()
        {
            InitializeEnemyLists();
            AutoRegisterExistingEnemies();
            
            // Debug stuff.
            Invoke("PrintEnemySummary", 0.5f);
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

        private void PrintEnemySummary()
        {
            for (int i = 0; i < enemiesPerRoom.Length; i++)
            {
                if (enemiesPerRoom[i].Count > 0)
                {
                    Debug.Log($"Room {i}: {enemiesPerRoom[i].Count} enemies registered");
                }
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
                if (registeredEnemies.Contains(enemy)) continue;

                Metro.Enemy.MetroEnemy metroEnemy = enemy.GetComponent<Metro.Enemy.MetroEnemy>();

                if (metroEnemy != null)
                {
                    int roomIndex = metroEnemy.roomIndex;

                    if (roomIndex >= 0 && roomIndex < enemiesPerRoom.Length)
                    {
                        RegisterEnemy(enemy, roomIndex);
                        registeredEnemies.Add(enemy); // Mark this enemy as registered
                    }
                    else
                    {
                        Debug.LogWarning($"Enemy '{enemy.name}' has an invalid room index ({roomIndex}). Skipping registration.");
                    }
                }
                else
                {
                    Debug.LogWarning($"Enemy '{enemy.name}' is missing the MetroEnemy script.");
                }
            }
        }

        private void CheckRoomProgress()
        {
            if (currentRoomIndex >= enemiesPerRoom.Length) return;

            if (enemiesPerRoom[currentRoomIndex].Count == 0 && !openedDoors.Contains(currentRoomIndex))
            {
                Debug.Log($"[IMPORTANT] All enemies defeated in room {currentRoomIndex}, opening door!");
                OpenDoor(currentRoomIndex);
                openedDoors.Add(currentRoomIndex);
            }
        }

        private void OpenDoor(int roomIndex)
        {
            if (roomIndex < metroDoors.Length)
            {
                doorAnimators[roomIndex].SetBool("doorCanOpen", true);
                Debug.Log($"[DOOR ACTION] Opening door {roomIndex}. Gameobject: {doorAnimators[roomIndex]}");
            }
        }

        private void CloseDoor(int roomIndex)
        {
            if (roomIndex < metroDoors.Length)
            {
                doorAnimators[roomIndex].SetBool("doorCanOpen", false);
                Debug.Log($"[DOOR ACTION] Closing door {roomIndex}");
            }
        }

        #endregion

        #region Public Methods

        public void RegisterEnemy(GameObject enemy, int roomIndex)
        {
            if (roomIndex < enemiesPerRoom.Length)
            {
                if (!enemiesPerRoom[roomIndex].Contains(enemy))
                {
                    enemiesPerRoom[roomIndex].Add(enemy);
                }
            }
        }

        public void EnemyDeactivated(GameObject enemy, int roomIndex)
        {
            // Preventing a bunch of debugs spawm.
            if (deactivatedEnemies.Contains(enemy)) return;
            
            if (roomIndex == currentRoomIndex && roomIndex < enemiesPerRoom.Length)
            {
                deactivatedEnemies.Add(enemy);
                enemiesPerRoom[roomIndex].Remove(enemy);
                
                // Only log key milestones.
                if (enemiesPerRoom[roomIndex].Count <= 3 && enemiesPerRoom[roomIndex].Count > 0)
                {
                    Debug.Log($"Room {roomIndex}: {enemiesPerRoom[roomIndex].Count} enemies remaining");
                }
                else if (enemiesPerRoom[roomIndex].Count == 0)
                {
                    Debug.Log($"Room {roomIndex}: All enemies defeated!");
                }

                CheckRoomProgress();
            }
        }

        public void PlayerEnteredNextRoom()
        {
            if (currentRoomIndex < metroDoors.Length)
            {
                CloseDoor(currentRoomIndex);
            }

            currentRoomIndex++;
            Debug.Log($"[PLAYER ACTION] Player entered room {currentRoomIndex}");
        }
        #endregion
    }
}