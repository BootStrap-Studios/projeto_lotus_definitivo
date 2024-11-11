using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    private List<GameObject> pooledObjects = new List<GameObject>();
    private int amountToPool;

    [SerializeField] private GameObject objectToPool;

    public GameObject GetPooledObject()
    {
        for(int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }

        return null;
    }

    public void DeterminaPool(int quantidadeInimigos, int quantidadeWaves)
    {
        if (quantidadeWaves <= 0) quantidadeWaves = 1;

        int quantidadePool = (quantidadeInimigos / quantidadeWaves) * 6;

        if (amountToPool == 0)
        {
            Debug.Log("Munições definidas para " + quantidadePool);
            amountToPool = quantidadePool;

            for (int i = 0; i < amountToPool; i++)
            {
                GameObject obj = Instantiate(objectToPool);
                obj.SetActive(false);
                pooledObjects.Add(obj);
            }
        }
        else if(amountToPool < quantidadePool)
        {
            Debug.Log("Aumentiei as munições de " + amountToPool + " para " + quantidadePool);

            int novoPool = quantidadePool - amountToPool;

            for (int i = 0; i < novoPool; i++)
            {
                GameObject obj = Instantiate(objectToPool);
                obj.SetActive(false);
                pooledObjects.Add(obj);
            }

            amountToPool = quantidadePool;
        }
    }
}
