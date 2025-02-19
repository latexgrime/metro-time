using Unity.VisualScripting;
using UnityEngine;

namespace Metro.Animation
{
    /// <summary>
    /// Manages the state of an individual metro door.
    /// Assign each door with this script.
    /// Controls opening and closing animations.
    /// </summary>
    
    public class MetroDoorState : MonoBehaviour
    {
        #region Variables
        [Header("- Animation")]
        [Tooltip("Assign the animator component controlling the door animation")]
        [SerializeField] private Animator doorAnimator;

        // private bool _isOpen = false;

        #endregion

        private void Start()
        {
            doorAnimator = GetComponent<Animator>();
        }

        #region Public Methods

        /// <summary>
        /// Opens the door if it's not already open
        /// </summary>
        public void OpenDoor()
        {
                doorAnimator.SetBool("doorCanOpen", true);
                // _isOpen = true;
        }

        /// <summary>
        /// Closes the door if it's not already closed.
        /// </summary>
        public void CloseDoor()
        {
                doorAnimator.SetBool("doorCanOpen", false);
                /// _isOpen = false;
        }

        #endregion
    }
}
