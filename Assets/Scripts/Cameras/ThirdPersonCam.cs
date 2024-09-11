using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCam : MonoBehaviour
{
    [Header("Variavéis/Referências")]
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform combatLookAt;
    [SerializeField] private Transform player;
    [SerializeField] private Transform playerObj;
    [SerializeField] private Rigidbody rb;

    [SerializeField] private float rotationSpeed;
    [SerializeField] private CameraStyle currentStyle;

    [Header("Cameras")]
    [SerializeField] private CinemachineFreeLook basicCamera; 
    [SerializeField] private CinemachineFreeLook combatCamera; 
    [SerializeField] private CinemachineFreeLook topDownCamera; 
    

    public enum CameraStyle
    {
        Basic,
        Combat,
        TopDown
    }


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        MyInputs();
        //CalculandoOrientacao();
        QualCamera();
    }

    private void MyInputs()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchCameraStyle(CameraStyle.Basic);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchCameraStyle(CameraStyle.Combat);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchCameraStyle(CameraStyle.TopDown);
    }

    private void CalculandoOrientacao()
    {
        //Rotate orientation
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;

    }

    private void QualCamera()
    {
        if(currentStyle == CameraStyle.Basic || currentStyle == CameraStyle.TopDown)
        {
            Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
            orientation.forward = viewDir.normalized;

            //Rotate player object
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

            if (inputDir != Vector3.zero)
            {
                playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
            }
        } else if(currentStyle == CameraStyle.Combat)
        {
            //Rotate orientation
            Vector3 viewDirCombat = combatLookAt.position - new Vector3(transform.position.x, combatLookAt.position.y, transform.position.z);
            orientation.forward = viewDirCombat.normalized;

            playerObj.forward = viewDirCombat.normalized;
        }
    }

    private void SwitchCameraStyle(CameraStyle newStyle)
    {
        basicCamera.Priority = 0;
        combatCamera.Priority = 0;
        topDownCamera.Priority = 0;

        if (newStyle == CameraStyle.Basic) basicCamera.Priority = 10;
        if (newStyle == CameraStyle.Combat) combatCamera.Priority = 10;
        if (newStyle == CameraStyle.TopDown) topDownCamera.Priority = 10;

        currentStyle = newStyle;

    }
}
