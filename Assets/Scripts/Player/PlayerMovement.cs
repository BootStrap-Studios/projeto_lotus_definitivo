using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class PlayerMovement : MonoBehaviour
{
    [Header("Ground Check")]
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask whatIsGround;
    bool grounded;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private Transform orientation;
    [SerializeField] private float groundDrag;
    [SerializeField] private CharacterController controller;

    [Header("Jump")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float airMultiplier;
    [SerializeField] private float gravidade;
    bool readyToJump;


    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    private void Start()
    {
        //rb = GetComponent<Rigidbody>();
        //rb.freezeRotation = true;
    }

    private void Update()
    {
        MyInput();
        GroundCheck();
        

        
        
    }

    private void FixedUpdate()
    {
        MovePlayer();
        //SpeedControl();
        //HandleDrag();
    }

    private void MyInput()
    {
        horizontalInput = UnityEngine.Input.GetAxisRaw("Horizontal");
        verticalInput = UnityEngine.Input.GetAxisRaw("Vertical");

        
    }

    private void GroundCheck()
    {
        //ground check
        //grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

  

        grounded = controller.isGrounded;
        
    }
    
    private void HandleDrag()
    {
        if(grounded)
        {
            rb.drag = groundDrag;
        } else
        {
            rb.drag = 0f;
        }
    }

    private void MovePlayer()
    {
        if (UnityEngine.Input.GetKey(KeyCode.Space) && grounded)
        {
            Jump();

            //Invoke(nameof(ResetJump), jumpCooldown);
        }


        //Calculate move direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        moveDirection.y += gravidade * Time.deltaTime;

        controller.Move(moveDirection.normalized * moveSpeed * Time.deltaTime);
        

    }

    private void SpeedControl()
    {
        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if(flatVelocity.magnitude > moveSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
        }
    }

    private void Jump()
    {
        Debug.Log("Pulei");
        //Reset velocity
        //rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        //rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        moveDirection.y += Mathf.Sqrt(jumpForce * -3.0f * gravidade);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
}
