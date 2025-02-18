using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Boss : MonoBehaviour
{
    public GameObject bullets;
    private int random;
    public Transform bulletPos;

    public Transform player;

    public float bulletVelocity = 10f;

    public float bulletSpeed;

    public Rigidbody2D bulletRb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(ShootPlayer(5));
    }

    IEnumerator ShootPlayer(float time)
    {
        //Vector3 shootingDirection = whichDirection().normalized;
        while (true)
        {
            GameObject bossBullet = Instantiate(bullets, bulletPos.position, Quaternion.identity);
           // bulletRb.linearVelocity = shootingDirection * bulletSpeed;
            // bullets[random].GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);
           // bossBullet.GetComponent<Rigidbody2D>().linearVelocity = whichDirection().normalized * bulletVelocity;
           Rigidbody2D rb = bossBullet.GetComponent<Rigidbody2D>();
           rb.linearVelocity = whichDirection() * bulletVelocity;
           
            yield return new WaitForSeconds(time);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Player"))
        {
            Destroy(bullets);
        }
    }

    private Vector2 whichDirection()
    {
        return (player.position - bulletPos.position).normalized;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
