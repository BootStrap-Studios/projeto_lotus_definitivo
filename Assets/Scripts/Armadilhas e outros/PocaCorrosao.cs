using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PocaCorrosao : MonoBehaviour
{
    private StatusJogador statusJogador;
    [SerializeField] private AudioSource source;

    private void Start()
    {
        statusJogador = FindObjectOfType<StatusJogador>();
        source.PlayOneShot(source.clip);

        Destroy(gameObject, statusJogador.duracaoPoca);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Inimigo"))
        {
            Debug.Log("Corroendo");
            other.GetComponent<Inimigo>().CorrosaoDireto();
        }
    }
}
