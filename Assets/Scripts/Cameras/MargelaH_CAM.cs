using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MargelaH_CAM : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera normalCAM;
    [SerializeField] private CinemachineVirtualCamera aimCAM;
    [SerializeField] private PlayerMovement player;
    [SerializeField] private GameObject playerUI;
    [SerializeField] private GameObject arma;

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
        aimCAM.gameObject.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            playerUI.SetActive(true);
            aimCAM.gameObject.SetActive(true);
            arma.SetActive(true);

            player.cameraCombate = true;

            normalCAM.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.Value = aimCAM.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.Value;
            normalCAM.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.Value = aimCAM.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.Value;
        }
        else
        {
            playerUI.SetActive(false);
            aimCAM.gameObject.SetActive(false);
            arma.SetActive(false);

            player.cameraCombate = false;

            aimCAM.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.Value = normalCAM.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.Value;
            aimCAM.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.Value = normalCAM.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.Value;
        }
    }

    private void PauseCam()
    {
        play = !play;

        aimCAM.enabled = play;
        normalCAM.enabled = play;
    }
}
