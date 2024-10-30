using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusJogador : MonoBehaviour
{
    [Header("Dano base das armas")]
    public float danoBasePistola;
    public float danoBaseShotgun;
    public float danoBaseShuriken;


    [Header("Dano atual das armas (Após modificadores)")]
    public float danoAtualPistola;
    public float danoAtualShotgun;
    public float danoAtualShuriken;


    [Header("Acerto crítico")]
    public float chanceDeAcertoCriticoBase = 0;
    public float chanceDeAcertoCriticoAtual = 0;

    public float danoDoAcertoCritico = 1.5f;


    private void Start()
    {
        Reset();
    }

    private void Reset()
    {
        
    }
}
