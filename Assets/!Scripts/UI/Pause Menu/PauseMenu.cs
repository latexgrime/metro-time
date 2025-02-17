using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Rendering.HighDefinition;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject settingsPanel;

    [Header("DLSS Settings")]
    public Toggle dlssToggle;
    public TMP_Dropdown dlssModeDropdown;
    public Toggle dlssOptimalToggle;
    public Slider dlssSharpnessSlider;
    public TMP_Text dlssSharpnessValueText;

    private HDAdditionalCameraData cameraData;
    private bool isPaused = false;

    void Start()
    {
        cameraData = UnityEngine.Camera.main.GetComponent<HDAdditionalCameraData>();

        pauseMenuUI.SetActive(false);
        settingsPanel.SetActive(false);

        // Load saved DLSS settings or set default values
        bool dlssEnabled = PlayerPrefs.GetInt("DLSS_Enabled", 0) == 1;
        dlssToggle.isOn = dlssEnabled;
        cameraData.allowDynamicResolution = dlssEnabled;
        cameraData.allowDeepLearningSuperSampling = dlssEnabled;

        int dlssMode = PlayerPrefs.GetInt("DLSS_Mode", 0);
        dlssModeDropdown.value = dlssMode;
        cameraData.deepLearningSuperSamplingQuality = (uint)dlssMode;

        bool optimalEnabled = PlayerPrefs.GetInt("DLSS_Optimal", 0) == 1;
        dlssOptimalToggle.isOn = optimalEnabled;
        cameraData.deepLearningSuperSamplingUseOptimalSettings = optimalEnabled;

        float dlssSharpness = PlayerPrefs.GetFloat("DLSS_Sharpness", 0.5f);
        dlssSharpnessSlider.value = dlssSharpness;
        cameraData.deepLearningSuperSamplingSharpening = dlssSharpness;

        // Ensure HDRP applies DLSS settings
        cameraData.deepLearningSuperSamplingUseCustomAttributes = true;

        // Update UI text for sharpness percentage
        dlssSharpnessValueText.text = $"{(dlssSharpness * 100):0}%";

        // Hide dropdown if "Use Optimal Settings" is enabled
        dlssModeDropdown.gameObject.SetActive(!optimalEnabled);

        Debug.Log("DLSS Settings Loaded & UI Updated.");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        settingsPanel.SetActive(false);
        Time.timeScale = 0f;
        isPaused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        settingsPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

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

        // Ensure Dynamic Resolution is enabled before applying DLSS
        cameraData.allowDynamicResolution = isEnabled;
        cameraData.allowDeepLearningSuperSampling = isEnabled;

        // Force HDRP to apply DLSS settings
        cameraData.deepLearningSuperSamplingUseCustomAttributes = true;

        PlayerPrefs.SetInt("DLSS_Enabled", isEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void ChangeDLSSMode()
    {
        int modeIndex = dlssModeDropdown.value;

        // Explicitly assign DLSS Quality Mode to force HDRP update.
        cameraData.deepLearningSuperSamplingQuality = (uint)modeIndex;
        cameraData.deepLearningSuperSamplingUseCustomQualitySettings = true;

        PlayerPrefs.SetInt("DLSS_Mode", modeIndex);
        PlayerPrefs.Save();
    }


    public void ToggleOptimalSettings()
    {
        bool isOptimal = dlssOptimalToggle.isOn;
        cameraData.deepLearningSuperSamplingUseOptimalSettings = isOptimal;

        PlayerPrefs.SetInt("DLSS_Optimal", isOptimal ? 1 : 0);
        PlayerPrefs.Save();

        // Hide dropdown when "Use Optimal Settings" is ON
        dlssModeDropdown.gameObject.SetActive(!isOptimal);
    }

    public void AdjustDLSSSharpness()
    {
        float sharpness = dlssSharpnessSlider.value;

        // Explicitly assign sharpness and force HDRP update.
        cameraData.deepLearningSuperSamplingSharpening = sharpness;
        cameraData.deepLearningSuperSamplingUseCustomAttributes = true;

        PlayerPrefs.SetFloat("DLSS_Sharpness", sharpness);
        PlayerPrefs.Save();

        // Update UI percentage.
        dlssSharpnessValueText.text = $"{(sharpness * 100):0}%";
    }

}
