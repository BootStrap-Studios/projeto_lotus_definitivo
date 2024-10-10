using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;
using Input = UnityEngine.Input;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float speedAndando;
    [SerializeField] float speedPulando;
    [SerializeField] float rotationSpeed;
    [SerializeField] CharacterController characterController;
    [SerializeField] float jumpSpeed;
    [SerializeField] Transform transformCamera;

    [SerializeField] private float dashTime;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashCooldown;

    public bool cameraCombate;

    float speed;
    float ySpeed;
    float originalStepOffSet;
    float dashCooldownAux;

    Vector3 movementDirection;

    private void Start()
    {
        originalStepOffSet = characterController.stepOffset;
    }

    private void Update()
    {
        Movimentacao();

        dashCooldownAux -= Time.deltaTime;

        if(Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownAux <= 0)
        {
            Dash();
            dashCooldownAux = dashCooldown;
        }
    }

    private void Movimentacao()
    {
        //Calculando a dire��o que o player tem que andar com base no input.
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        movementDirection = new Vector3(horizontalInput, 0, verticalInput);
        float magnitude = Mathf.Clamp01(movementDirection.magnitude) * speed;

        movementDirection = Quaternion.AngleAxis(transformCamera.rotation.eulerAngles.y, Vector3.up) * movementDirection;
        movementDirection.Normalize();

        //Checando que o personagem est� no ch�o e o input para poder pular.
        if (characterController.isGrounded)
        {
            speed = speedAndando;
            characterController.stepOffset = originalStepOffSet;
            ySpeed = -0.5f;

            if (Input.GetButtonDown("Jump"))
            {
                ySpeed = jumpSpeed;
            }
        }
        else
        {
            characterController.stepOffset = 0;
            speed = speedPulando;
        }

        //Aplicando o pulo no axis y, juntamente do calculo da gravidade.
        ySpeed += Physics.gravity.y * Time.deltaTime;

        Vector3 velocity = movementDirection * magnitude;
        velocity.y = ySpeed;

        characterController.Move(velocity * Time.deltaTime);

        //Mudando a rota��o do personagem.
        if (movementDirection != Vector3.zero)
        {
            //Se a camera de combate estiver ativa, n�o queremos que o codigo mude a rota��o, j� que a camera estar� fazendo isso.
            if (!cameraCombate)
            {
                Debug.Log("Rotacionando Player");
                Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
            }
        }

        if (cameraCombate)
        {
            Quaternion toRotation = Quaternion.Euler(0f, transformCamera.eulerAngles.y, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void Dash()
    {
        StartCoroutine(DashCoroutine());
    }

    private IEnumerator DashCoroutine()
    {
        float startTime = Time.time;

        while(Time.time < startTime + dashTime)
        {
            characterController.Move(movementDirection * dashSpeed * Time.deltaTime);

            yield return null;
        }
    }

    public void MudaCharacterController()
    {
        characterController.enabled = !characterController.enabled;
    }
}
