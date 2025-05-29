using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjetilInimigo : MonoBehaviour
{
    [SerializeField] private Rigidbody rbProjetil;
    [SerializeField] private float tempoAtivo;
    [SerializeField] private TrailRenderer rastroProjetil;
    Vector3 positionInicial;
    private float velProjetil;   
    private float tempoAtivoAux;
    private float danoProjetil;


    private void OnTriggerEnter(Collider other)
    {      
        if (other.CompareTag("Player"))
        {
            try
            {
                other.GetComponent<VidaPlayer>().TomarDano(danoProjetil);
                rastroProjetil.time = 0;
                gameObject.SetActive(false);
            }
            catch
            {
                //nada
            }
        }
        else
        {
            rastroProjetil.time = 0;
            transform.position = positionInicial;
            gameObject.SetActive(false);
        }
    }


    void Update()
    {
        rbProjetil.velocity = transform.forward * velProjetil;

        tempoAtivoAux -= Time.deltaTime;

        if(tempoAtivoAux <= 0)
        {
            rastroProjetil.time = 0;
            transform.position = positionInicial;
            gameObject.SetActive(false);
            return;
        }

        if(tempoAtivoAux < tempoAtivo)
        {
            rastroProjetil.time = 0.3f;
        }
    }

    public void InstanciaProjetil(float dano, Transform pontaArma, float velocidadeProjetil)
    {
        tempoAtivoAux = tempoAtivo;
        transform.position = pontaArma.position;
        positionInicial = pontaArma.position;
        danoProjetil = dano;
        transform.rotation = pontaArma.rotation;
        velProjetil = velocidadeProjetil;
    }
}
