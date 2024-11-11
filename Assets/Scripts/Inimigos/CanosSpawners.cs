using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CanosSpawners : MonoBehaviour
{
    [SerializeField] private SpawnInimigos spawnInimigos;
    [SerializeField] private GameObject plataforma;
    [SerializeField] private Transform posFinal;    
    [SerializeField] private float vel;
    [SerializeField] private float tempoAtivando;
    [SerializeField] private Transform[] posSpawn;
    private float tempoAtivandoAux;
    private List<Inimigo> inimigosSpawnados;
    private bool acionando;
    private int qualPosAtivar;

    public void AtivaInimigo(int numInimigo)
    {
        if (qualPosAtivar == 0) inimigosSpawnados = new List<Inimigo>();

        inimigosSpawnados.Add(spawnInimigos.inimigosVivos[numInimigo].GetComponent<Inimigo>());

        inimigosSpawnados[qualPosAtivar].gameObject.transform.position = posSpawn[qualPosAtivar].position;
        inimigosSpawnados[qualPosAtivar].gameObject.transform.rotation = gameObject.transform.rotation;

        qualPosAtivar++;

        tempoAtivandoAux = tempoAtivando;
        acionando = true;
    }

    private void Update()
    {
        if (acionando)
        {
            tempoAtivandoAux -= Time.deltaTime;

            plataforma.transform.position = Vector3.Slerp(plataforma.transform.position, posFinal.position, vel * Time.deltaTime);

            for (int i = 0; i < inimigosSpawnados.Count; i++)
            {
                inimigosSpawnados[i].peInimigo.transform.position = plataforma.transform.position;
                inimigosSpawnados[i].gameObject.transform.position = new Vector3(inimigosSpawnados[i].gameObject.transform.position.x, inimigosSpawnados[i].peInimigo.transform.position.y + 1, inimigosSpawnados[i].gameObject.transform.position.z);
            }

            if (tempoAtivandoAux < 0)
            {
                acionando = false;

                qualPosAtivar = 0;

                plataforma.transform.position = gameObject.transform.position;

                for (int i = 0; i < inimigosSpawnados.Count; i++)
                {
                    inimigosSpawnados[i].enabled = true;
                    inimigosSpawnados[i].GetComponent<NavMeshAgent>().enabled = true;
                }                
            }
        }
    }
}
