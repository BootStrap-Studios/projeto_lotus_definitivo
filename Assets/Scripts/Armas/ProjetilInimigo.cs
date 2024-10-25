using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjetilInimigo : MonoBehaviour
{
    [SerializeField] private Rigidbody rbProjetil;
    [SerializeField] private float velProjetil;
    [SerializeField] private float tempoAtivo;
    private float tempoAtivoAux;
    private float danoProjetil;


    private void OnTriggerEnter(Collider other)
    {      
        if (other.CompareTag("Player"))
        {
            other.GetComponent<VidaPlayer>().TomarDano(danoProjetil);
            gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("Inimigo errou tiro");
            gameObject.SetActive(false);
        }
    }


    void Update()
    {
        rbProjetil.velocity = transform.forward * velProjetil;

        tempoAtivoAux -= Time.deltaTime;

        if(tempoAtivoAux < 0)
        {
            gameObject.SetActive(false);
        }
    }

    public void InstanciaProjetil(float dano, Vector3 pontaArma)
    {
        tempoAtivoAux = tempoAtivo;
        transform.position = pontaArma;
        danoProjetil = dano;
    }
}
