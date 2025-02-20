using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Rendering.HighDefinition;

public class PauseMenu : MonoBehaviour
{
    [Header("- UI References")]
    public GameObject pauseMenuUI;
    public GameObject settingsPanel;

    [Header("- DLSS Settings")]
    public Toggle dlssToggle;
    public TMP_Dropdown dlssModeDropdown;
    public Toggle dlssOptimalToggle;
    public Slider dlssSharpnessSlider;
    public TMP_Text dlssSharpnessValueText;

    private HDAdditionalCameraData _cameraData;
    private bool _isPaused = false;

    void Start()
    {
        InitializeReferences();
        SetupInitialUI();
        LoadDLSSSettings();
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
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        pauseMenuUI.SetActive(true);
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

        dlssSharpnessValueText.text = $"Sharpness: {(sharpness * 100):0}%";
    }
}