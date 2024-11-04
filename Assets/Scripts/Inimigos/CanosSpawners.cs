using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanosSpawners : MonoBehaviour
{
    public void SpawnandoInimigo(GameObject inimigo)
    {
        Instantiate(inimigo, gameObject.transform);
    }
}
