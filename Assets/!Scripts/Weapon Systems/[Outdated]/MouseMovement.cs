using System;
using UnityEngine;
using Unity.Cinemachine;

public class MouseMovement : MonoBehaviour
{
    private float rotationX = 0f;

    private float rotationY = 0f;

    public float sensitivity = 15f;

    public float topClamp = -90f;
    public float bottomClamp = 90f;

    private void Start()
    {
        //Getting rid of the cursor in game
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    private void Update()
    {
        LookAround();
    }

    private void LookAround()
    {
        //Mouse inputs
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        
        //Rotation to look up and down
        rotationX -= mouseY;
        
        //Clamp the rotation so that when we move the mouse too much to the top, the rotation is blocked
        rotationX = Mathf.Clamp(rotationX, topClamp, bottomClamp);
        
        //Rotation to look left and right
        rotationY += mouseX;
        
        //Apply rotation to our transform
        transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0f);
    }
}
