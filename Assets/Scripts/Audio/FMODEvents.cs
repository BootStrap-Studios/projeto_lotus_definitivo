using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Teste")]
    [field: SerializeField] public EventReference teste { get; private set; }

    public static FMODEvents instance { get; private set; }

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
        }

        instance = this;
    }
}
