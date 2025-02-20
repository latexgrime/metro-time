using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CinematicToGameplay : MonoBehaviour
{
    public GameObject imageOne, imageTwo, lastImage;
    private void Start()
    {
        imageTwo.SetActive(false);
        lastImage.SetActive(false);
    }

    public void ImageOneToImageTwo()
    {
        imageTwo.SetActive(true);
    }

    public void ImageTwoToLastImage()
    {
        lastImage.SetActive(true);
    }
    
    public void SendPlayerToGameplay()
    {
        SceneManager.LoadScene("Gameplay");
    }
}
