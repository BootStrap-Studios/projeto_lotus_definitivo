using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PocaCorrosao : MonoBehaviour
{
    private StatusJogador statusJogador;
    

    private void Start()
    {
        statusJogador = FindObjectOfType<StatusJogador>();

        AudioManager.instance.PlayOneShot(FMODEvents.instance.spawnPoca, transform.position);

        Destroy(gameObject, statusJogador.duracaoPoca);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Inimigo"))
        {
            if(other.GetComponent<Inimigo>() != null)
            {
                Debug.Log("Corroendo");
                other.GetComponent<Inimigo>().CorrosaoDireto();
            }  
        }
    }
}
