using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CanosSpawners : MonoBehaviour
{
    [SerializeField] private SpawnInimigos spawnInimigos;
    [SerializeField] private GameObject plataforma;
    [SerializeField] private Transform posFinal;
    [SerializeField] private Transform posSpawn;
    [SerializeField] private float vel;
    [SerializeField] private float tempoAtivando;
    private Inimigo inimigosSpawnado;
    private float tempoAtivandoAux;  
    private bool acionando;

    public void AtivaInimigo(int numInimigo)
    {        
        inimigosSpawnado = spawnInimigos.inimigosVivos[numInimigo].GetComponentInChildren<Inimigo>();

        inimigosSpawnado.gameObject.transform.position = posSpawn.position;
        inimigosSpawnado.gameObject.transform.rotation = gameObject.transform.rotation;

        tempoAtivandoAux = tempoAtivando;
        acionando = true;
    }

    private void Update()
    {
        if (acionando)
        {
            tempoAtivandoAux -= Time.deltaTime;

            plataforma.transform.position = Vector3.Slerp(plataforma.transform.position, posFinal.position, vel * Time.deltaTime);

            inimigosSpawnado.peInimigo.transform.position = plataforma.transform.position;
            inimigosSpawnado.gameObject.transform.position = new Vector3(inimigosSpawnado.gameObject.transform.position.x, inimigosSpawnado.peInimigo.transform.position.y + 1, inimigosSpawnado.gameObject.transform.position.z);

            if (tempoAtivandoAux < 0)
            {
                acionando = false;

                plataforma.transform.position = gameObject.transform.position;

                inimigosSpawnado.enabled = true;
                inimigosSpawnado.GetComponent<NavMeshAgent>().enabled = true;
            }
        }
    }
}
