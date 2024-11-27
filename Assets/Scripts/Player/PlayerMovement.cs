using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;
using TMPro;
using Input = UnityEngine.Input;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimentação Player")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform transformCamera;
    [SerializeField] private float rotationSpeed;    
    [SerializeField] private float jumpSpeed;
    public bool cameraCombate;

    [Header("Dash")]
    [SerializeField] private float dashTime;
    [SerializeField] private float dashSpeed;
    [SerializeField] public float dashCooldown;
    [SerializeField] private GameObject dashObj;
    [SerializeField] private Image dashImage;

    [Header("TPs")]
    public Vector3 posInstituto;

    private float speed;
    private float ySpeed;
    private float originalStepOffSet;
    private float dashCooldownAux;

    private Vector3 movementDirection;

    private StatusJogador statusJogador;

    [SerializeField] private Animator animator;

    private bool tp;

    private void OnEnable()
    {
        EventBus.Instance.onTP += MudaCharacterController;
    }

    private void OnDisable()
    {
        EventBus.Instance.onTP -= MudaCharacterController;
    }

    private void Start()
    {
        originalStepOffSet = characterController.stepOffset;

        statusJogador = FindObjectOfType<StatusJogador>();       
    }

    private void Update()
    {
        if (tp)
        {
            animator.SetBool("Correndo", false);
        }
        else
        {
            Movimentacao();

            if (dashCooldownAux < dashCooldown)
            {
                dashCooldownAux += Time.deltaTime;
                dashImage.fillAmount = dashCooldownAux / dashCooldown;
            }
            else
            {
                dashImage.fillAmount = (float)statusJogador.quantidadeDeDash / statusJogador.quantidadeDeDashTotal;
            }

            if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownAux >= dashCooldown)
            {
                Dash();

                if (statusJogador.quantidadeDeDash <= 0)
                {
                    dashCooldownAux = 0;
                    statusJogador.quantidadeDeDash = statusJogador.quantidadeDeDashTotal;
                }
            }

            if (Input.GetKeyDown(KeyCode.F) && statusJogador.tenhoULT && statusJogador.statusULT >= 10)
            {
                Debug.Log("ULTEEEEEEEEEEEEEEEEEEEEEI");
                statusJogador.Ultando();
            }
        }
    }


    #region Funções da Movimentação do Player
    private void Movimentacao()
    {
        //Calculando a direção que o player tem que andar com base no input.
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        movementDirection = new Vector3(horizontalInput, 0, verticalInput);
        float magnitude = Mathf.Clamp01(movementDirection.magnitude) * speed;

        movementDirection = Quaternion.AngleAxis(transformCamera.rotation.eulerAngles.y, Vector3.up) * movementDirection;
        movementDirection.Normalize();

        if(movementDirection != Vector3.zero)
        {
            animator.SetBool("Correndo", true);
        } else {
            animator.SetBool("Correndo", false);
        }

        //Checando que o personagem está no chão e o input para poder pular.
        if (characterController.isGrounded)
        {
            speed = statusJogador.velocidadeAndando;
            characterController.stepOffset = originalStepOffSet;
            ySpeed = -0.5f;

            if (Input.GetButtonDown("Jump"))
            {
                ySpeed = jumpSpeed;
                animator.SetTrigger("Pulo");
            }
        }
        else
        {
            characterController.stepOffset = 0;
            speed = statusJogador.velocidadePulando;
        }

        //Aplicando o pulo no axis y, juntamente do calculo da gravidade.
        ySpeed += Physics.gravity.y * Time.deltaTime;

        Vector3 velocity = movementDirection * magnitude;
        velocity.y = ySpeed;

        characterController.Move(velocity * Time.deltaTime);

        //Mudando a rotação do personagem.
        if (movementDirection != Vector3.zero)
        {
            //Se a camera de combate estiver ativa, não queremos que o codigo mude a rotação, já que a camera estará fazendo isso.
            if (!cameraCombate)
            {
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

    private void MudaCharacterController(bool characterContorller, bool mexer)
    {
        characterController.enabled = characterContorller;
        tp = mexer;
    }

    #endregion

    #region Funções do Dash
    private void Dash()
    {
        StartCoroutine(DashCoroutine());
        statusJogador.quantidadeDeDash -= 1;
        AudioManager.instance.PlayOneShot(FMODEvents.instance.dashMavie, transform.position);
    }

    private IEnumerator DashCoroutine()
    {
        float startTime = Time.time;

        dashObj.SetActive(true);

        animator.SetTrigger("Dash");

        //Conferindo se os buffs de dash de defesa ou corrosao estao ativos;
        if(statusJogador.dashDefesaAtivo)
        {
            statusJogador.AtivarEscudo();
        }
        if(statusJogador.dashCorrosaoAtivo)
        {
            statusJogador.SpawnarPocaCorrosao();
        }

        while(Time.time < startTime + dashTime)
        {
            characterController.Move(movementDirection * dashSpeed * Time.deltaTime);

            yield return null;
        }

        dashObj.SetActive(false);

    }

    #endregion

}
