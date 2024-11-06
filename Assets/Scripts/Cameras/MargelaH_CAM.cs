using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MargelaH_CAM : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera normalCAM;
    [SerializeField] private CinemachineVirtualCamera aimCAM;
    [SerializeField] private PlayerMovement player;
    [SerializeField] private GameObject armaUI;
    [SerializeField] private GameObject[] armas;
    [SerializeField] private GameObject pontaDaArma;
    [SerializeField] private Animator animator;

    private AmmoSystem ammoSystem;
    private GameObject armaAtual;
    private int numArma;
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
            armaUI.SetActive(true);
            aimCAM.gameObject.SetActive(true);
            armaAtual.SetActive(true);

            player.cameraCombate = true;

            animator.SetBool("Atirando", true);

            normalCAM.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.Value = aimCAM.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.Value;
            normalCAM.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.Value = aimCAM.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.Value;
        }
        else
        {
            if(ammoSystem.municaoAtual < ammoSystem.municaoTotal)
            {
                armaUI.SetActive(true);
            }
            else
            {
                armaUI.SetActive(false);
            }

            aimCAM.gameObject.SetActive(false);
            armaAtual.SetActive(false);

            player.cameraCombate = false;

            aimCAM.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.Value = normalCAM.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.Value;
            aimCAM.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.Value = normalCAM.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.Value;

            animator.SetBool("Atirando", false);

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Debug.Log("Você trocou o modo de disparo para: PISTOLA");
                TrocarArma(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Debug.Log("Você trocou o modo de disparo para: SHOTGUN");
                TrocarArma(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Debug.Log("Você trocou o modo de disparo para: SHURIKEN");
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
    }

    public void AplicarSensi(float sensiOlhar, float sensiMirar)
    {
        normalCAM.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_MaxSpeed = sensiOlhar;
        normalCAM.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_MaxSpeed = sensiOlhar;

        aimCAM.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_MaxSpeed = sensiMirar;
        aimCAM.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_MaxSpeed = sensiMirar;
    }
}
