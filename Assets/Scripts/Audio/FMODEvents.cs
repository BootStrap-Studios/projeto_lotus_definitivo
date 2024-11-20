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

    [field: Header("SpawnPoca")]
    [field: SerializeField] public EventReference spawnPoca { get; private set; }
    
    [field: Header("BurstProc")]
    [field: SerializeField] public EventReference burstProc { get; private set; }
    
    [field: Header("TerminouSala")]
    [field: SerializeField] public EventReference terminouSala { get; private set; }

    [field: Header("PauseGame")]
    [field: SerializeField] public EventReference pauseGame { get; private set; }

    [field: Header("AbrirTerminalBuffs")]
    [field: SerializeField] public EventReference abrirTerminalBuffs { get; private set; }

    [field: Header("AmbienteFabrica")]
    [field: SerializeField] public EventReference ambienteFabrica { get; private set; }

    [field: Header("MusicaFabrica")]
    [field: SerializeField] public EventReference musicaFabrica { get; private set; }

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
