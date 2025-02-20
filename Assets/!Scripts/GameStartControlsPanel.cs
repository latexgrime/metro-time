using _Scripts.Camera;
using _Scripts.Player;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Scripts
{
    public class GameStartControlsPanel : MonoBehaviour
    {
        [Header("- UI References")]
        [SerializeField] private GameObject controlsPanel;
        [SerializeField] private Button continueButton;

        [Header("- Settings")]
        [SerializeField] private bool showOnlyFirstTime = true;
        [SerializeField] private string hasSeenControlsPanelKey = "HasSeenControlsPanel";

        private InputManager _inputManager;
        private PlayerCamera _playerCamera;
        private bool _hasInitialized = false;

        private void Awake()
        {

        }

        private void Start()
        {
            InitializeReferences();
            Invoke(nameof(ShowControlsPanel), 0.25f);
        }

        private void InitializeReferences()
        {
            _inputManager = FindFirstObjectByType<InputManager>();
            _playerCamera = FindFirstObjectByType<PlayerCamera>();

            if (continueButton != null)
            {
                continueButton.onClick.AddListener(ContinueGame);
            }
            
            _hasInitialized = true;
        }

        private void ShowControlsPanel()
        {
            // Make sure we have all dependencies
            if (!_hasInitialized)
            {
                InitializeReferences();
            }

            // Pause game time
            Time.timeScale = 0f;

            // Show mouse cursor 
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            // Show the controls panel
            if (controlsPanel != null)
            {
                controlsPanel.SetActive(true);
                
                // Set initial selection for gamepad input
                if (EventSystem.current != null && continueButton != null)
                {
                    EventSystem.current.SetSelectedGameObject(continueButton.gameObject);
                }
            }
        }

        public void ContinueGame()
        {
            // Resume game time
            Time.timeScale = 1f;

            // Hide cursor and lock it
            if (_playerCamera != null)
            {
                _playerCamera.MakeCursorInvisible();
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }

            // Store that player has seen the controls panel
            if (showOnlyFirstTime)
            {
                PlayerPrefs.SetInt(hasSeenControlsPanelKey, 1);
                PlayerPrefs.Save();
            }

            // Hide the panel
            if (controlsPanel != null)
                controlsPanel.SetActive(false);
            
            // Optional: destroy this component
            Destroy(gameObject);
        }

        // For skip button or pressing ESC
        private void Update()
        {
            if (controlsPanel != null && controlsPanel.activeSelf)
            {
                if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space))
                {
                    ContinueGame();
                }
            }
        }
    }
}