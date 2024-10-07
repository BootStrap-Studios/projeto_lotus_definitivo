using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static Cinemachine.CinemachineCore;
using static StateInimigos;

public class Inimigo : MonoBehaviour
{
    [Header("Outros")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform pontaArma;

    [Header("Arma")]
    [SerializeField] private int municao;
    [SerializeField] private float danoTiro;
    [SerializeField] private float cooldownTiro;
    [SerializeField] private float alcanceArma;
    public int municaoAux;

    [Header("Vida")]
    [SerializeField] private BarraDeVida _barraDeVida;
    [SerializeField] private float vida; 
    private float vidaAtual;   
    
    private NavMeshAgent agent;
    private StateInimigos stateInimigo;
    
    

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        vidaAtual = vida;
        _barraDeVida.AlterarBarraDeVida(vidaAtual, vida);
        municaoAux = municao;

        stateInimigo = new Idle(gameObject, agent, player, municao, alcanceArma, vida, cooldownTiro); 
    }

    void Update()
    {
        stateInimigo = stateInimigo.Process();
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
        municaoAux--;

        if (municaoAux <= 0)
        {
            //stateInimigo.reload = true;
        }
        else
        {            
            RaycastHit hit;

            if(Physics.Raycast(pontaArma.position, transform.TransformDirection(Vector3.forward), out hit, alcanceArma + 3))
            {
                if(hit.transform.tag == "Player")
                {
                    hit.transform.GetComponentInParent<VidaPlayer>().TomarDano(danoTiro);
                }
                else
                {
                    //Debug.Log("pegou em nada");
                }
            }
        }
    }
}
