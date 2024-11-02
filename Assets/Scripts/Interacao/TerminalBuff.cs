using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerminalBuff : MonoBehaviour, IInteractable
{
    private BuffManager buffManager;
    void Start()
    {
        buffManager = FindObjectOfType<BuffManager>();
    }

    public void Interagir()
    {
        buffManager.SorteandoQualArvore();
    }


    
}
