using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PocaCorrosao : MonoBehaviour
{
    private StatusJogador statusJogador;

    private void Start()
    {
        statusJogador = FindObjectOfType<StatusJogador>();

        Destroy(gameObject, statusJogador.duracaoPoca);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Inimigo"))
        {
            other.GetComponent<Inimigo>().CorrosaoDireto();
        }
    }
}
