using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class TP_Player : MonoBehaviour, IInteractable
{
    [SerializeField] private Vector3 posTP;
    [SerializeField] private SpawnInimigos spawn;
    [SerializeField] private CinemachineVirtualCamera cameraPlayer;
    [SerializeField] private bool tpFinal;
    [SerializeField] private GameObject fimUI;
    private PlayerMovement player;

    private StatusJogador statusJogador;
    VidaPlayer vidaPlayer;


    private void Start()
    {
        statusJogador = FindObjectOfType<StatusJogador>();
        vidaPlayer = FindObjectOfType<VidaPlayer>();
    }
    public void Interagir()
    {
        if (tpFinal)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            fimUI.SetActive(true);
        }
        else
        {
            player = FindObjectOfType<PlayerMovement>();
            EventBus.Instance.FadeIn(0.5f, DarTP);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (tpFinal)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                fimUI.SetActive(true);
            }
            else
            {
                player = other.GetComponent<PlayerMovement>();
                EventBus.Instance.FadeIn(0.5f, DarTP);
            }
        }
    }

    public void DarTP()
    {
        player.GetComponent<PlayerMovement>().MudaCharacterController();

        CuraPorSala();

        if(spawn != null)
        { 
            player.gameObject.transform.position = spawn.gameObject.transform.position;
            player.gameObject.transform.rotation = spawn.gameObject.transform.rotation;
            spawn.RanomizandoInimigos();

            //Debug.Log(spawn.gameObject.transform.eulerAngles.y);
            cameraPlayer.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.Value = spawn.gameObject.transform.eulerAngles.y;
            cameraPlayer.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.Value = 0;
        }
        else
        {
            player.gameObject.transform.position = posTP;
        }
        
        EventBus.Instance.FadeOut(0.5f, player.GetComponent<PlayerMovement>().MudaCharacterController);
    }

    public void JogarNovamente()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        fimUI.SetActive(false);
        SceneManager.LoadScene("Implemenetacao");
    }

    private void CuraPorSala()
    {
        vidaPlayer.CurarVida(statusJogador.curaPorSala);
    }
}
