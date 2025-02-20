using UnityEngine;
using _Scripts.Boss;
using UnityEngine.Events;

public class BossDefeated : MonoBehaviour
{
    public GameObject bossDefeatedImage;
    [SerializeField] public GameObject bossPrefab;

    //[SerializeField] public BossHealthUI bossHealth;

    public UnityEvent bossDefeatedEvent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bossDefeatedImage.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
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
