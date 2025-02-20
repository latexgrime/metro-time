using System;
using NUnit.Framework;
using UnityEngine;

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
        Debug.Log("Send player to image 2");
        imageOne.SetActive(false);
        imageTwo.SetActive(true);
    }

    public void ImageTwoToLastImage()
    {
        Debug.Log("Send player to last image");
        imageTwo.SetActive(false);
        lastImage.SetActive(true);
    }
    
    public void SendPlayerToGameplay()
    {
        Debug.Log("Send player to gameplay scene");
    }
}
