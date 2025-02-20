using System;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;

    [SerializeField] private float remainingTime;

    public UnityEvent timerEvent;

    public AudioSource source;
    
    public AudioClip timerSoundFinished;
    private bool soundPlayed = false;

    private void Start()
    { 
        source.Play();
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
        }
        else if (remainingTime <= 0 && !soundPlayed)
        {
            remainingTime = 0;
            timerText.color = Color.red;
            TimerSound();
            soundPlayed = true;
        }

        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    public void RestartGame()
    {
        //STUFF HAPPENING FOR SURE
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