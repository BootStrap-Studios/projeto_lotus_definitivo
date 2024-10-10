using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TP_Player : MonoBehaviour
{
    [SerializeField] private Vector3 posTP;
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
        player.gameObject.transform.position = posTP;

        EventBus.Instance.FadeOut(0.5f, player.GetComponent<PlayerMovement>().MudaCharacterController);
    }
}
