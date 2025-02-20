using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Rendering.HighDefinition;
using _Scripts.Camera;

public class PauseMenu : MonoBehaviour
{
    [Header("- UI References")]
    public GameObject pauseMenuUI;
    public GameObject settingsPanel;
    public GameObject dlssSettingsPanel;
    public GameObject cameraSettingsPanel;

    [Header("- DLSS Settings")]
    public Toggle dlssToggle;
    public TMP_Dropdown dlssModeDropdown;
    public Toggle dlssOptimalToggle;
    public Slider dlssSharpnessSlider;
    public TMP_Text dlssSharpnessValueText;

    [Header("- Camera Settings")]
    public Slider sensitivityXSlider;
    public Slider sensitivityYSlider;
    public TMP_Text sensitivityXValueText;
    public TMP_Text sensitivityYValueText;
    public Button resetCameraSettingsButton;

    private HDAdditionalCameraData _cameraData;
    private PlayerCamera _playerCamera;
    private bool _isPaused = false;

    void Start()
    {
        InitializeReferences();
        SetupInitialUI();
        LoadDLSSSettings();
        LoadCameraSettings();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    private void InitializeReferences()
    {
        _cameraData = UnityEngine.Camera.main.GetComponent<HDAdditionalCameraData>();
        _playerCamera = UnityEngine.Camera.main.GetComponent<PlayerCamera>();
        
        if (_playerCamera == null)
        {
            Debug.LogWarning("PlayerCamera component not found on main camera!");
        }
    }

    private void SetupInitialUI()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        if (dlssSettingsPanel != null)
        {
            dlssSettingsPanel.SetActive(false);
        }

        if (cameraSettingsPanel != null)
        {
            cameraSettingsPanel.SetActive(false);
        }
    }

    private void LoadDLSSSettings()
    {
        // Load DLSS enabled setting.
        bool dlssEnabled = PlayerPrefs.GetInt("DLSS_Enabled", 0) == 1;
        dlssToggle.isOn = dlssEnabled;
        _cameraData.allowDynamicResolution = dlssEnabled;
        _cameraData.allowDeepLearningSuperSampling = dlssEnabled;

        // Load DLSS quality mode.
        int dlssMode = PlayerPrefs.GetInt("DLSS_Mode", 0);
        dlssModeDropdown.value = dlssMode;
        _cameraData.deepLearningSuperSamplingQuality = (uint)dlssMode;

        // Load optimal settings preference.
        bool optimalEnabled = PlayerPrefs.GetInt("DLSS_Optimal", 0) == 1;
        dlssOptimalToggle.isOn = optimalEnabled;
        _cameraData.deepLearningSuperSamplingUseOptimalSettings = optimalEnabled;

        // Load sharpness value.
        float dlssSharpness = PlayerPrefs.GetFloat("DLSS_Sharpness", 0.5f);
        dlssSharpnessSlider.value = dlssSharpness;
        _cameraData.deepLearningSuperSamplingSharpening = dlssSharpness;

        // Enable custom attributes.
        _cameraData.deepLearningSuperSamplingUseCustomAttributes = true;

        // Update UI display.
        dlssSharpnessValueText.text = $"{(dlssSharpness * 100):0}%";
        dlssModeDropdown.gameObject.SetActive(!optimalEnabled);
    }

    private void LoadCameraSettings()
    {
        if (_playerCamera == null) return;

        Vector2 sensitivity = _playerCamera.GetSensitivityValues();
        
        // Set slider values
        sensitivityXSlider.value = sensitivity.x;
        sensitivityYSlider.value = sensitivity.y;
        
        // Update text displays
        UpdateSensitivityXText(sensitivity.x);
        UpdateSensitivityYText(sensitivity.y);
    }

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        settingsPanel.SetActive(false);
        Time.timeScale = 0f;
        _isPaused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        settingsPanel.SetActive(false);
        Time.timeScale = 1f;
        _isPaused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        // Default to first tab (DLSS settings)
        OpenDLSSSettings();
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        cameraSettingsPanel.SetActive(false);
        dlssSettingsPanel.SetActive(false);
        cameraSettingsPanel.SetActive(false);
        pauseMenuUI.SetActive(true);
    }
    
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f; // Ensure time is reset before loading menu
        SceneManager.LoadScene("MainMenu"); // Replace "MainMenu" with your actual menu scene name
    }

    public void OpenDLSSSettings()
    {
        dlssSettingsPanel.SetActive(true);
        cameraSettingsPanel.SetActive(false);
    }

    public void OpenCameraSettings()
    {
        dlssSettingsPanel.SetActive(false);
        cameraSettingsPanel.SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // DLSS Settings
    public void ToggleDlss()
    {
        bool isEnabled = dlssToggle.isOn;

        _cameraData.allowDynamicResolution = isEnabled;
        _cameraData.allowDeepLearningSuperSampling = isEnabled;
        _cameraData.deepLearningSuperSamplingUseCustomAttributes = true;

        PlayerPrefs.SetInt("DLSS_Enabled", isEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void ChangeDLSSMode()
    {
        int modeIndex = dlssModeDropdown.value;

        _cameraData.deepLearningSuperSamplingQuality = (uint)modeIndex;
        _cameraData.deepLearningSuperSamplingUseCustomQualitySettings = true;

        PlayerPrefs.SetInt("DLSS_Mode", modeIndex);
        PlayerPrefs.Save();
    }

    public void ToggleOptimalSettings()
    {
        bool isOptimal = dlssOptimalToggle.isOn;
        _cameraData.deepLearningSuperSamplingUseOptimalSettings = isOptimal;

        PlayerPrefs.SetInt("DLSS_Optimal", isOptimal ? 1 : 0);
        PlayerPrefs.Save();

        dlssModeDropdown.gameObject.SetActive(!isOptimal);
    }

    public void AdjustDLSSSharpness()
    {
        float sharpness = dlssSharpnessSlider.value;

        _cameraData.deepLearningSuperSamplingSharpening = sharpness;
        _cameraData.deepLearningSuperSamplingUseCustomAttributes = true;

        PlayerPrefs.SetFloat("DLSS_Sharpness", sharpness);
        PlayerPrefs.Save();

        dlssSharpnessValueText.text = $"{(sharpness * 100):0}%";
    }

    // Camera Sensitivity Settings
    public void AdjustSensitivityX()
    {
        if (_playerCamera == null) return;
        
        float value = sensitivityXSlider.value;
        _playerCamera.SetXSensitivityValue(value);
        
        UpdateSensitivityXText(value);
        SaveCameraSensitivitySettings();
    }
    
    public void AdjustSensitivityY()
    {
        if (_playerCamera == null) return;
        
        float value = sensitivityYSlider.value;
        _playerCamera.SetYSensitivityValue(value);
        
        UpdateSensitivityYText(value);
        SaveCameraSensitivitySettings();
    }
    
    private void UpdateSensitivityXText(float value)
    {
        if (sensitivityXValueText != null)
        {
            sensitivityXValueText.text = $"Horizontal Sensitivity: {(value * 100):0}%";
        }
    }
    
    private void UpdateSensitivityYText(float value)
    {
        if (sensitivityYValueText != null)
        {
            sensitivityYValueText.text = $"Vertical Sensitivity: {(value * 100):0}%";
        }
    }
    
    public void ResetCameraSettings()
    {
        if (_playerCamera == null) return;
        
        _playerCamera.SetDefaultSensitivityValues();
        Vector2 defaultValues = _playerCamera.GetSensitivityValues();
        
        // Update sliders
        sensitivityXSlider.value = defaultValues.x;
        sensitivityYSlider.value = defaultValues.y;
        
        // Update text
        UpdateSensitivityXText(defaultValues.x);
        UpdateSensitivityYText(defaultValues.y);
        
        SaveCameraSensitivitySettings();
    }
    
    private void SaveCameraSensitivitySettings()
    {
        Vector2 sensitivity = _playerCamera.GetSensitivityValues();
        PlayerPrefs.SetFloat("Camera_Sensitivity_X", sensitivity.x);
        PlayerPrefs.SetFloat("Camera_Sensitivity_Y", sensitivity.y);
        PlayerPrefs.Save();
    }
}