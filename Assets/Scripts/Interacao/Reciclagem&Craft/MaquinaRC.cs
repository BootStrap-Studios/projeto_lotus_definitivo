using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaquinaRC : MonoBehaviour, IInteractable
{
    [SerializeField] private MaquinaRCManager maquinaRCManager;
    [SerializeField] private Collider trigger;

    public void Interagir()
    {
        EventBus.Instance.PodePausar(false);
        EventBus.Instance.PauseGame();

        trigger.enabled = false;
        maquinaRCManager.stateRC = MaquinaRCManager.StateRC.inicio;
        maquinaRCManager.maquinaRC_UI.SetActive(true);
        maquinaRCManager.AtualizaInventario();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0;
    }
}
