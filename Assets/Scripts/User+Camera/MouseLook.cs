using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{

    public float mouseSensitivity = 1000f;

    public Transform playerBody;
    public Animator playerAnimator;

    private float mouseXSmooth = 0;

    float xRotation = 0f;
    public bool disableAnimation;

    void Start()
    {
        disableAnimation = false;
    }

    private void OnGUI()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        mouseXSmooth = Mathf.Lerp(mouseXSmooth, Input.GetAxis("Mouse X"), 4 * Time.deltaTime);
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        
        if(!disableAnimation)
            playerAnimator.SetFloat("Turn", mouseXSmooth);
        else
            playerAnimator.SetFloat("Turn", 0);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
