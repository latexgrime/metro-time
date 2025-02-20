using UnityEngine;
using UnityEngine.SceneManagement;
public class SendToMainMenu : MonoBehaviour
{
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
