using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MesaDeBuffsPermanente : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject buffsPermnentesUI;
    [SerializeField] private Collider trigger;
    private BuffsPermanenteManager buffsPermanenteManager;

    private void Awake()
    {
        buffsPermanenteManager = buffsPermnentesUI.GetComponent<BuffsPermanenteManager>();
    }

    public void Interagir()
    {
        EventBus.Instance.PauseGame();

        trigger.enabled = false;      
        buffsPermnentesUI.SetActive(true);
        buffsPermanenteManager.AtualizaInventario();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0;
    }
}
