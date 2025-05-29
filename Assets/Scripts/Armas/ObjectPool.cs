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
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
            else if(i + 1 >= pooledObjects.Count)
            {
                GameObject obj = Instantiate(objectToPool);
                obj.SetActive(false);
                pooledObjects.Add(obj);
            }
        }

        return null;
    }

    public void DeterminaPool(GameObject[] inimigo)
    {
        for (int i = 0; i < inimigo.Length; i++)
        {
            amountToPool += 3;
        }

        for (int i = 0; i < amountToPool; i++)
        {
            GameObject obj = Instantiate(objectToPool);
            obj.SetActive(false);
            pooledObjects.Add(obj);
        }
    }

    public void FimFase()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            Destroy(pooledObjects[i]);
        }

        pooledObjects = new List<GameObject>();
    }
}
