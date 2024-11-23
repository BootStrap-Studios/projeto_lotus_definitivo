using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MesaDeBuffsPermanente : MonoBehaviour, IInteractable
{
    [SerializeField] private Collider trigger;
    private BuffsPermanenteManager buffsPermanenteManager;

    private void Awake()
    {
        buffsPermanenteManager = FindObjectOfType<BuffsPermanenteManager>();
    }

    public void Interagir()
    {
        EventBus.Instance.PodePausar(false);
        EventBus.Instance.PauseGame();

        buffsPermanenteManager.scrollbar.value = 0.999f;

        trigger.enabled = false;
        buffsPermanenteManager.buffsPermanenteUI.SetActive(true);
        buffsPermanenteManager.uiLigada = true;
        buffsPermanenteManager.AtualizaInventario();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0;
    }
}
