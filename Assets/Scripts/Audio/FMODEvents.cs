using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Passos")]
    [field: SerializeField] public EventReference passos { get; private set; }

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
