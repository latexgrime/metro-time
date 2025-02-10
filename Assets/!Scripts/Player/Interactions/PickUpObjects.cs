using System;
using NALEO._Scripts;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class PickUpObjects : MonoBehaviour
{
    private AudioSource _audioSource;
    private InputManager _inputManager;

    [Header("- Item interaction")] 
    [SerializeField] private float launchForce = 25f;
    [SerializeField] private float objectInteractionDistance = 3f;
    [SerializeField] private float heldObjectPositionDistance = 2f;
    /// <summary>
    ///     The force intensity applied to the object when picked up.
    /// </summary>
    [SerializeField] private float movingObjectForce = 500f;
    /// <summary>
    ///     Setting a large number for this so the amount of force applied when picking up an object doesn't break the
    ///     immersion.
    /// </summary>
    [SerializeField] private float heldObjectDragTarget = 25f;
    [SerializeField] private string pickableTag = "CanPickUp";

    private GameObject _heldObject;
    private Rigidbody _heldObjectRb;
    private float _heldObjectDefaultDrag;
    private float _heldObjectHeight;
    private bool _isThrowing;
    private bool _isHolding = false;
    
    [Header("- SFX")] 
    [SerializeField] private AudioClip throwObjectSfx;
    
    private float _defaultVolume;
    private float _defaultPitch;

    private void Start()
    {
        GetComponents();
    }

    // Get the required components.
    private void GetComponents()
    {
        _inputManager = FindFirstObjectByType<InputManager>();
        _audioSource = GetComponent<AudioSource>();
        _defaultVolume = _audioSource.volume;
        _defaultPitch = _audioSource.pitch;
    }

    private void Update()
    {
        HandleObjectInteraction();
    }

    private void FixedUpdate()
    {
        HandleHeldObjectPhysics();
    }
    
    [SerializeField] private float pickUpCooldown = 0.1f;
    private float _lastPickUpTime;

    private void HandleObjectInteraction()
    {
        if (_heldObject)
        {
            if (_inputManager.interactInput && _isHolding && Time.time > _lastPickUpTime + pickUpCooldown) 
            {
                DropObject();
                _isHolding = false;
            }
        
            if (_inputManager.throwInput)
                _isThrowing = true;
        }
        else
        {
            if (_inputManager.interactInput && !_isHolding)
            {
                AttemptPickUpObject();
                _isHolding = true;
                _lastPickUpTime = Time.time;
            }
        }

        if (!_inputManager.interactInput)
        {
            _isHolding = false;
        }
    }

    private void DropObject()
    {
        if (!_heldObject) return;
        
        _heldObjectRb.linearDamping = _heldObjectDefaultDrag;
        _heldObjectRb.useGravity = true;
        
        _heldObject = null;
        _heldObjectRb = null;
    }

    private void AttemptPickUpObject()
    {
        if (Physics.Raycast(transform.position, transform.forward, out var hit, objectInteractionDistance))
        {
            if (hit.transform.CompareTag(pickableTag))
            {
                PickUpObject(hit.transform.gameObject);
            }
        }
    }

    private void PickUpObject(GameObject objectToPickUp)
    {
        _heldObject = objectToPickUp;
        _heldObjectRb = objectToPickUp.GetComponent<Rigidbody>();
        
        // Save the original drag value the object had to apply it later when the player drops the object.
        _heldObjectDefaultDrag = _heldObjectRb.linearDamping;
        
        // Set the drag to the drag target value. This is to fix the problem with the object receiving too much force when picked up.
        _heldObjectRb.linearDamping = heldObjectDragTarget;
        _heldObjectRb.useGravity = false;
    }

    private void HandleHeldObjectPhysics()
    {
        if (_heldObject != null)
        {
            var heldObjectRb = _heldObject.GetComponent<Rigidbody>();
            var moveObjectTo =
                transform.position + heldObjectPositionDistance * transform.forward + _heldObjectHeight * transform.up;
            var positionDifference = moveObjectTo - _heldObject.transform.position;

            // Set the position of the grabbed object to be in front of the player.
            heldObjectRb.AddForce(positionDifference * movingObjectForce);

            // Set the rotation of the object to be the same as the player.
            _heldObject.transform.rotation = transform.rotation;

            if (_isThrowing)
            {
                heldObjectRb.linearDamping = _heldObjectDefaultDrag;
                heldObjectRb.useGravity = true;
                heldObjectRb.AddForce(transform.forward * launchForce);
                _heldObject = null;
                _isThrowing = !_isThrowing;
                _audioSource.pitch = Random.Range(_defaultPitch - 0.1f, _defaultPitch + 0.1f);
                _audioSource.volume = Random.Range(_defaultVolume - 0.2f, _defaultVolume + 0.2f);
                _audioSource.PlayOneShot(throwObjectSfx);
            }
        }
    }
}