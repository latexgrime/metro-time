using UnityEngine;
using UnityEngine.SceneManagement;

namespace MainMenu.Buttons
{
    /// <summary>
    /// Contains All Main Menu Buttons' functionality.
    /// </summary>

    public class MainMenuButtonFunctions : MonoBehaviour
    {
        #region Variables
        [SerializeField] private GameObject optionsPanel;
        [SerializeField] private GameObject aboutPanel;
        #endregion

        public void Play()
        {
            SceneManager.LoadScene(1);
        }
        
        public void ToggleOptionsPanel()
        {
            optionsPanel.SetActive(!optionsPanel.activeSelf);
        }
        
        public void ToggleAboutPanel()
        {
            aboutPanel.SetActive(!aboutPanel.activeSelf);
        }
        
        public void Exit()
        {
            Application.Quit();
        }
    }
}