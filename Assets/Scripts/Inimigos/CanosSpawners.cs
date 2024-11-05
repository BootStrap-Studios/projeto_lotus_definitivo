using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanosSpawners : MonoBehaviour
{
    [SerializeField] private SpawnInimigos spawnInimigos;  

    public void SpawnandoInimigo(GameObject inimigo, int i)
    {
        spawnInimigos.inimigosVivos[i] = Instantiate(inimigo, gameObject.transform.position, gameObject.transform.rotation);
        //spawnInimigos.inimigosVivos[i].SetActive(false);
    }
}
