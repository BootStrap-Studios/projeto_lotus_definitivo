using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MesaDeBuffsPermanente : MonoBehaviour, IInteractable
{
    [SerializeField] private Collider trigger;
    [SerializeField] private BuffsPermanenteManager buffsPermanenteManager;

    public void Interagir()
    {
        EventBus.Instance.PodePausar(false);       

        buffsPermanenteManager.scrollbar.value = 0.999f;

        trigger.enabled = false;
        buffsPermanenteManager.buffsPermanenteUI.SetActive(true);
        buffsPermanenteManager.uiLigada = true;
        buffsPermanenteManager.AtualizaInventario();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        EventBus.Instance.PauseGame();
        Time.timeScale = 0;
    }
}
