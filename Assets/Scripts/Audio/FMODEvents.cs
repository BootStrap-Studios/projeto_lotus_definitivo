using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Passos")]
    [field: SerializeField] public EventReference passos { get; private set; }

    [field: Header("Tiro escopeta")]
    [field: SerializeField] public EventReference manoEsco { get; private set; }

    [field: Header("Tiro pistola")]
    [field: SerializeField] public EventReference manoPistol { get; private set; }

    [field: Header("Tiro shuriken")]
    [field: SerializeField] public EventReference manoShuri { get; private set; }

    [field: Header("Morte")]
    [field: SerializeField] public EventReference morte { get; private set; }

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
