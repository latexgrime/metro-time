using UnityEngine;
using _Scripts.Boss;
using UnityEngine.Events;

public class BossDefeated : MonoBehaviour
{
    public GameObject bossDefeatedImage;
    [SerializeField] public GameObject bossPrefab;

    public UnityEvent bossDefeatedEvent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bossDefeatedImage.SetActive(false);
    }

    public void SendPlayerToMenu()
    {
        Debug.Log("Send player to menu");
    }

    public void BossHasBeenKilled()
    {
        bossDefeatedEvent.Invoke();
    }
}
