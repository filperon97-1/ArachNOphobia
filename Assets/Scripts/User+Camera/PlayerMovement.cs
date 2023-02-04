using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public CharacterController controller;

    public float speed = 12f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    public bool isGrounded;

    Vector3 velocity;

    public Animator playerAnimator;

    public bool isGrabbing;

    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        playerAnimator.SetBool("walking", false);
        isGrabbing = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(groundCheck != null)
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        else 
        {
            Vector3 groundPos = new Vector3(0.0f, 0.0f, 0.0f);
            isGrounded = Physics.CheckSphere(groundPos, groundDistance, groundMask);
        }

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
            
        controller.Move(move * speed * Time.deltaTime);

        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        UpdateAnimation(x , z);

    }

    private void UpdateAnimation(float x , float z)
    {
        if (isGrabbing)     //animazione camminata normale
        {
            //z = 1;
            z = x * x + z * z;
            x = 0;
        }
        playerAnimator.SetBool("walking", x!=0 || z!=0);
        playerAnimator.SetFloat("Xdir", x);
        playerAnimator.SetFloat("Zdir", z);
    }
}
