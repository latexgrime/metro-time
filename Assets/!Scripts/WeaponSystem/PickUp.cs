using System;
using Dyson.Weapons;
using UnityEngine;
using Random = UnityEngine.Random;

public class PickUp : MonoBehaviour
{
    public Weapon gunReference;
    public Rigidbody rb;
    public CapsuleCollider capsuleColl;

    public Transform player, gunContainer, cameraReference;
    public float pickUpRange;
    public float dropForwardForce, dropUpwardForce;
    
    public bool isEquipped;
    public static bool slotFull;


    private void Start()
    {
        if (!isEquipped)
        {
            gunReference.enabled = false;
            rb.isKinematic = false;
            capsuleColl.isTrigger = false;
        }

        if (isEquipped)
        {
            gunReference.enabled = true;
            rb.isKinematic = true;
            capsuleColl.isTrigger = true;
            slotFull = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 distanceToPlayer = player.position - transform.position;
        if (!isEquipped && distanceToPlayer.magnitude <= pickUpRange && Input.GetKeyDown(KeyCode.E) && !slotFull)
        {
            PickUpWeapon();
        }

        if (isEquipped && Input.GetKeyDown(KeyCode.Q))
        {
            DropWeapon();
        }
    }

    private void PickUpWeapon()
    {
        isEquipped = true;
        slotFull = true;
        
        transform.SetParent(gunContainer);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        transform.localScale = Vector3.one;
        
        rb.isKinematic = true;
        capsuleColl.isTrigger = true;

        gunReference.enabled = true;
    }

    private void DropWeapon()
    {
        isEquipped = false;
        slotFull = false;
        
        transform.SetParent(null);
        
        rb.isKinematic = false;
        capsuleColl.isTrigger = false;

        rb.linearVelocity = player.GetComponent<Rigidbody>().linearVelocity;
        
        rb.AddForce(cameraReference.forward * dropForwardForce, ForceMode.Impulse);
        rb.AddForce(cameraReference.up * dropUpwardForce, ForceMode.Impulse);

        float random = Random.Range(-1f, 1f);
        rb.AddTorque(new Vector3(random, random, random) * 10);
        gunReference.enabled = false;
    }
}
