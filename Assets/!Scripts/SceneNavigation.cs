using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles scene loading operations with support for both index and name-based loading.
/// Can be attached to any GameObject that needs scene transition functionality.
/// </summary>
public class SceneNavigation : MonoBehaviour
{
    [Header("- Optional Settings")]
    [Tooltip("Optional loading screen to show during scene transitions")]
    [SerializeField] private GameObject loadingScreen;
    
    [Tooltip("Whether to show debug logs for scene loading")]
    [SerializeField] private bool showDebugLogs = true;

    /// <summary>
    /// Loads a scene by its build index.
    /// </summary>
    /// <param name="sceneIndex">The build index of the scene to load</param>
    public void LoadSceneByIndex(int sceneIndex)
    {
        if (showDebugLogs) 
            Debug.Log($"Loading scene with index: {sceneIndex}");
            
        ShowLoadingScreenIfAvailable();
        SceneManager.LoadScene(sceneIndex);
    }

    /// <summary>
    /// Loads a scene by its name.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load</param>
    public void LoadSceneByName(string sceneName)
    {
        if (showDebugLogs)
            Debug.Log($"Loading scene with name: {sceneName}");
            
        ShowLoadingScreenIfAvailable();
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Loads the next scene in the build index.
    /// Wraps around to the first scene if at the end.
    /// </summary>
    public void LoadNextScene()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = (currentIndex + 1) % SceneManager.sceneCountInBuildSettings;
        
        LoadSceneByIndex(nextIndex);
    }

    /// <summary>
    /// Reloads the current active scene.
    /// </summary>
    public void ReloadCurrentScene()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        LoadSceneByIndex(currentIndex);
    }

    /// <summary>
    /// Loads the first scene in the build settings (typically the main menu).
    /// </summary>
    public void LoadMainMenu()
    {
        LoadSceneByIndex(0);
    }

    /// <summary>
    /// Displays the loading screen if one is assigned.
    /// </summary>
    private void ShowLoadingScreenIfAvailable()
    {
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true);
        }
    }

    /// <summary>
    /// Loads a scene asynchronously by index.
    /// </summary>
    /// <param name="sceneIndex">The build index of the scene to load</param>
    public void LoadSceneAsyncByIndex(int sceneIndex)
    {
        StartCoroutine(LoadSceneAsyncCoroutine(sceneIndex));
    }

    /// <summary>
    /// Loads a scene asynchronously by name.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load</param>
    public void LoadSceneAsyncByName(string sceneName)
    {
        StartCoroutine(LoadSceneAsyncCoroutine(sceneName));
    }

    /// <summary>
    /// Coroutine for asynchronous scene loading by index.
    /// </summary>
    private System.Collections.IEnumerator LoadSceneAsyncCoroutine(int sceneIndex)
    {
        if (showDebugLogs)
            Debug.Log($"Loading scene asynchronously with index: {sceneIndex}");
            
        ShowLoadingScreenIfAvailable();
        
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
        
        // Wait until the scene is fully loaded
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    /// <summary>
    /// Coroutine for asynchronous scene loading by name.
    /// </summary>
    private System.Collections.IEnumerator LoadSceneAsyncCoroutine(string sceneName)
    {
        if (showDebugLogs)
            Debug.Log($"Loading scene asynchronously with name: {sceneName}");
            
        ShowLoadingScreenIfAvailable();
        
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        
        // Wait until the scene is fully loaded
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    /// <summary>
    /// Quits the application. Works in builds but not in the editor.
    /// </summary>
    public void QuitGame()
    {
        if (showDebugLogs)
            Debug.Log("Quitting application");
            
        Application.Quit();
        
        // This line helps for testing in the editor
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}