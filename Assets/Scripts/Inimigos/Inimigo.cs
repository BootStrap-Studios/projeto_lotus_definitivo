using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using static Cinemachine.CinemachineCore;
using static StateInimigos;

public class Inimigo : MonoBehaviour
{
    [Header("Outros")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform pontaArma;
    [SerializeField] private ObjectPool objectPool;
    private ProjetilInimigo tiro;
    public LayerMask playerMask;
    private NavMeshAgent agent;
    private StateInimigos stateInimigo;

    [Header("Arma & Personagem")]
    [SerializeField] private int municao;
    public int municaoAux;
    [SerializeField] private float danoTiro;
    [SerializeField] private float alcanceMaxArma;
    [SerializeField] private float alcanceMinArma;
    [SerializeField] private float cooldownTiro;
    [SerializeField] private float velocidadeAndar;
    public bool inimigoExplosivo;
    

    [Header("UI_Inimigos")]
    [SerializeField] private BarraDeVida _barraDeVida;
    [SerializeField] private float vida; 
    [SerializeField] private TextMeshProUGUI statusInimigo; 
    private float vidaAtual;            

    void Start()
    {
        //player = FindObjectOfType<PlayerMovement>().transform;
        agent = GetComponent<NavMeshAgent>();
        vidaAtual = vida;
        _barraDeVida.AlterarBarraDeVida(vidaAtual, vida);
        municaoAux = municao;

        stateInimigo = new Idle(gameObject, agent, player, municao, alcanceMaxArma, alcanceMinArma, cooldownTiro, velocidadeAndar); 
    }

    void Update()
    {
        stateInimigo = stateInimigo.Process();
    }

    public void TomarDano(float dano)
    {
        if (vidaAtual == vida)
        {
            stateInimigo.stunar = true;
        }

        vidaAtual -= dano;
        _barraDeVida.AlterarBarraDeVida(vidaAtual, vida);

        if (vidaAtual <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void Atirar()
    {
        if (municaoAux <= 0)
        {
            stateInimigo.reload = true;
        }
        else
        {            
            RaycastHit hit;

            if(!inimigoExplosivo && Physics.Raycast(pontaArma.position, transform.TransformDirection(Vector3.forward), out hit, alcanceMaxArma + 3))
            {

                if (hit.transform.tag == "Player")
                {
                    player.GetComponentInParent<VidaPlayer>().TomarDano(danoTiro);
                    //tiro = objectPool.GetPooledObject().GetComponent<ProjetilInimigo>();
                    //tiro.gameObject.SetActive(true);
                    //tiro.InstanciaProjetil(danoTiro, pontaArma.position);
                }
                else
                {
                   // Debug.Log(hit.collider.gameObject.name);
                }
            }
            else if(inimigoExplosivo && Physics.CheckSphere(transform.position, alcanceMinArma, playerMask))
            {
                player.GetComponentInParent<VidaPlayer>().TomarDano(danoTiro);
                Destroy(gameObject);
            }
            else if(inimigoExplosivo && !Physics.CheckSphere(transform.position, alcanceMinArma, playerMask))
            {
                Destroy(gameObject);
            }
        }

        municaoAux--;
    }

    public void AtualizaStatus(string statusAtual)
    {
        statusInimigo.text = statusAtual;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, alcanceMaxArma);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, alcanceMinArma);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 20f);
    }
}
