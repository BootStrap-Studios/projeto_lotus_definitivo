using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using static Cinemachine.CinemachineCore;
using static StateInimigos;

public class Inimigo : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform pontaArma;
    [SerializeField] private int municao;
    private int municaoAux;
    [SerializeField] private float danoTiro;
    [SerializeField] private float cooldownTiro;
    [SerializeField] private float alcanceArma;
    [SerializeField] private float vida;
    private float vidaAtual;   
    [SerializeField] private BarraDeVida _barraDeVida;
    private NavMeshAgent agent;
    private StateInimigos stateAtual;
    
    

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        vidaAtual = vida;
        _barraDeVida.AlterarBarraDeVida(vidaAtual, vida);
        municaoAux = municao;

        stateAtual = new Idle(gameObject, agent, player, municao, alcanceArma, vida, cooldownTiro); 
    }

    void Update()
    {
        stateAtual = stateAtual.Process();
    }

    public void TomarDano(float dano)
    {
        vidaAtual -= dano;
        _barraDeVida.AlterarBarraDeVida(vidaAtual, vida);

        if(vidaAtual <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void Atirar()
    {
        if (municaoAux <= 0)
        {
            municaoAux = municao;
            stateAtual = new Reload(gameObject, agent, player, municao, alcanceArma, vida, cooldownTiro);
            stateAtual.stage = EVENT.EXIT;
        }
        else
        { 
            municaoAux--;

            RaycastHit hit;

            if(Physics.Raycast(pontaArma.position, transform.TransformDirection(Vector3.forward), out hit, alcanceArma + 3))
            {
                if(hit.transform.tag == "Player")
                {
                    hit.transform.GetComponentInParent<VidaPlayer>().TomarDano(danoTiro);
                }
                else
                {
                    Debug.Log("pegou em nada");
                }
            }
        }
    }
}
