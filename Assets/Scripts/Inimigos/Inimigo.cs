using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Inimigo : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private int municao;
    [SerializeField] private float alcanceArma;
    [SerializeField] private float vida;
    [SerializeField] private float cooldownTiro;
    private NavMeshAgent agent;
    private StateInimigos stateAtual;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        Debug.Log(stateAtual);
        stateAtual = new Idle(gameObject, agent, player, municao, alcanceArma, vida, cooldownTiro);
    }

    void Update()
    {
        stateAtual = stateAtual.Process();
    }
}
