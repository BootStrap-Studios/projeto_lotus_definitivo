using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class TP_Player : MonoBehaviour
{
    [SerializeField] private Vector3 posTP;
    [SerializeField] private SpawnInimigos spawn;
    [SerializeField] private CinemachineVirtualCamera cameraPlayer;
    private PlayerMovement player;



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.GetComponent<PlayerMovement>();
            EventBus.Instance.FadeIn(0.5f, DarTP);
        }
    }

    public void DarTP()
    {
        player.GetComponent<PlayerMovement>().MudaCharacterController();

        try
        {
            player.gameObject.transform.position = spawn.gameObject.transform.position;
            player.gameObject.transform.rotation = spawn.gameObject.transform.rotation;
            spawn.RanomizandoInimigos();

            //Debug.Log(spawn.gameObject.transform.eulerAngles.y);
            cameraPlayer.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.Value = spawn.gameObject.transform.eulerAngles.y;
            cameraPlayer.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.Value = 0;
        }
        catch
        {
            player.gameObject.transform.position = posTP;
        }
        
        EventBus.Instance.FadeOut(0.5f, player.GetComponent<PlayerMovement>().MudaCharacterController);
    }
}
