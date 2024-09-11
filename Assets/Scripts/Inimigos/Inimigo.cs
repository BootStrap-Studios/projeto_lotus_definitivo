using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Inimigo : MonoBehaviour
{
    NavMeshAgent agent;
    public Transform player;
    StateInimigos stateAtual;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        Debug.Log(stateAtual);
        stateAtual = new Idle(gameObject, agent, player);
    }

    void Update()
    {
        stateAtual = stateAtual.Process();
    }
}
