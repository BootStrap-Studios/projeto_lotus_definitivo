using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanosSpawners : MonoBehaviour
{
    [SerializeField] private SpawnInimigos spawnInimigos;  

    public void SpawnandoInimigo(GameObject inimigo, int i)
    {
        float posSpawnX = Random.Range(gameObject.transform.position.x - 2, gameObject.transform.position.x + 2);
        float posSpawnZ = Random.Range(gameObject.transform.position.z - 2, gameObject.transform.position.z + 2);

        Vector3 posSpawn = new Vector3(posSpawnX, gameObject.transform.position.y, posSpawnZ);

        spawnInimigos.inimigosVivos[i] = Instantiate(inimigo, posSpawn, gameObject.transform.rotation);
        //spawnInimigos.inimigosVivos[i].SetActive(false);
    }
}
