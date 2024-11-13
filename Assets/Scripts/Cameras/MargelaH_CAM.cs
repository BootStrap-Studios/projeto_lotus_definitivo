using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class MargelaH_CAM : MonoBehaviour
{
    [Header("Armas")]
    [SerializeField] private GameObject miraUI;
    [SerializeField] private GameObject[] armas;
    [SerializeField] private GameObject[] armasUI;
    [SerializeField] private TextMeshProUGUI armasDesc;

    [Header("Outros")]
    [SerializeField] private CinemachineVirtualCamera normalCAM;
    [SerializeField] private CinemachineVirtualCamera aimCAM;
    [SerializeField] private PlayerMovement player;   
    [SerializeField] private GameObject pontaDaArma;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject crosshair;

    private AmmoSystem ammoSystem;
    private GameObject armaAtual;
    
    private bool play = true;
     
    private void OnEnable()
    {
        EventBus.Instance.onPauseGame += PauseCam;
    }
    private void OnDisable()
    {
        EventBus.Instance.onPauseGame -= PauseCam;
    }

    void Start()
    {
        ammoSystem = FindObjectOfType<AmmoSystem>();

        aimCAM.gameObject.SetActive(false);
        armaAtual = armas[0];

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            miraUI.SetActive(true);
            crosshair.SetActive(true);
            aimCAM.gameObject.SetActive(true);
            armaAtual.SetActive(true);

            player.cameraCombate = true;

            animator.SetBool("Atirando", true);

            Animacao();

            normalCAM.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.Value = aimCAM.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.Value;
            normalCAM.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.Value = aimCAM.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.Value;
        }
        else
        {
            if(ammoSystem.municaoAtual < ammoSystem.municaoTotal)
            {
                miraUI.SetActive(true);
            }
            else
            {
                miraUI.SetActive(false);
            }


            crosshair.SetActive(false);
            aimCAM.gameObject.SetActive(false);
            armaAtual.SetActive(false);

            player.cameraCombate = false;

            aimCAM.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.Value = normalCAM.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.Value;
            aimCAM.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.Value = normalCAM.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.Value;

            animator.SetBool("Atirando", false);

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                TrocarArma(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                TrocarArma(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                TrocarArma(2);
            }
        }

        Vector3 newRotation = new Vector3(Camera.main.transform.eulerAngles.x, Camera.main.transform.eulerAngles.y, Camera.main.transform.eulerAngles.z);
        pontaDaArma.transform.eulerAngles = newRotation;
    }

    private void PauseCam()
    {  
        play = !play;

        aimCAM.enabled = play;
        normalCAM.enabled = play;
    }

    private void TrocarArma(int num)
    {
        armaAtual = armas[num];

        for(int i = 0; i < armasUI.Length; i++)
        {
            if(i == num)
            {
                armasUI[i].SetActive(true);
            }
            else
            {
                armasUI[i].SetActive(false);
            }
        }

        switch (num)
        {
            case 0:
                armasDesc.text = "Pistola";
                break;

            case 1:
                armasDesc.text = "Shotgun";
                break;

            case 2:
                armasDesc.text = "Shurikens";
                break;
        }
            
    }

    private void Animacao()
    {
        float valorX = aimCAM.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.Value;
        float valorY = aimCAM.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.Value;

        animator.SetFloat("x", valorX);
        animator.SetFloat("y", valorY);
    }


    public void AplicarSensi(float sensiOlhar, float sensiMirar)
    {
        normalCAM.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_MaxSpeed = sensiOlhar;
        normalCAM.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_MaxSpeed = sensiOlhar;

        aimCAM.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_MaxSpeed = sensiMirar;
        aimCAM.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_MaxSpeed = sensiMirar;
    }
}
