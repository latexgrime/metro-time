using System;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;

    [SerializeField] private float remainingTime;

    public UnityEvent timerEvent;

    public AudioSource source;
    
    public AudioClip timerSoundFinished;
    private bool soundPlayed = false;

    public GameObject gameOver;

    private void Start()
    { 
        source.Play();
        gameOver.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        TimerDecreasing();
    }

    public void TimerDecreasing()
    {
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            if (remainingTime <= 60)
            {
                timerText.color = Color.red;
            }
        }
        else if (remainingTime <= 0 && !soundPlayed)
        {
            remainingTime = 0;
            TimerSound();
            soundPlayed = true;
            gameOver.SetActive(true);
        }
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    public void RestartGame()
    {
        SceneManager.LoadScene("Gameplay");
    }

    public void TimerSound()
    {
        if (source.isPlaying)
        {
            source.Stop();
            source.clip = timerSoundFinished;
            source.Play();
            Invoke("StopSound", source.clip.length);
        }
    }

    private void StopSound()
    {
        source.Stop();
    }

}