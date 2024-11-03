using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevador : MonoBehaviour, IInteractable
{
    private ManagerSalas manager;

    private void Awake()
    {
        manager = FindObjectOfType<ManagerSalas>();
    }
    public void Interagir()
    {
        manager.IrPraProxSala();
    }
}
