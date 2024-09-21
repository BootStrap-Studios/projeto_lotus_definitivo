using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Inimigo : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private int municao;
    [SerializeField] private float alcanceArma;
    [SerializeField] private int vida;
    [SerializeField] private float cooldownTiro;
    [SerializeField] private BarraDeVida _barraDeVida;
    private NavMeshAgent agent;
    private StateInimigos stateAtual;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        stateAtual = new Idle(gameObject, agent, player, municao, alcanceArma, vida, cooldownTiro);

        _barraDeVida.AlterarBarraDeVida(vida, vida);
    }

    void Update()
    {
        stateAtual = stateAtual.Process();

        if (Input.GetKeyDown(KeyCode.T))
        {
            TomarDano();
        }
    }

    public void TomarDano()
    {
        stateAtual = new TomarDano(gameObject, agent, player, municao, alcanceArma, vida, cooldownTiro);
    }
}
