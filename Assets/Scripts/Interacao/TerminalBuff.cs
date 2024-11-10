using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class TerminalBuff : MonoBehaviour, IInteractable
{
    private BuffManager buffManager;

    [SerializeField] private AudioSource source;
    void Start()
    {
        buffManager = FindObjectOfType<BuffManager>();
    }

    public void Interagir()
    {
        buffManager.SorteandoQualArvore();
        source.PlayOneShot(source.clip);
        gameObject.GetComponent<BoxCollider>().enabled = false;
    }


    
}
