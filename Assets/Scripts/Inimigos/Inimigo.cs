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
    [SerializeField] private GameObject dropPrefab;
    private ProjetilInimigo tiro;
    public LayerMask playerMask;
    private NavMeshAgent agent;
    private StateInimigos stateInimigo;

    [Header("Arma")]
    [SerializeField] private int municao;
    [SerializeField] private float danoTiro;
    [SerializeField] private float danoTiroReduzido;
    [SerializeField] private float alcanceMaxArma;
    [SerializeField] private float alcanceMinArma;
    [SerializeField] private float cooldownTiro;
    [SerializeField] private float velProjetil;
    

    [Header("Personagem")]
    [SerializeField] private float velocidadeAndar;
    public bool inimigoExplosivo;

    [Header("UI_Inimigos")]
    [SerializeField] private BarraDeVida _barraDeVida;
    [SerializeField] private float vida; 
    [SerializeField] private TextMeshProUGUI statusInimigo; 
    private float vidaAtual;


    [Header("Status")]
    public float danoCorrosao = 1f;
    private int statusCorrosao = 0;
    private bool boolCorrosao = false; 

    private int statusMovimentacao = 0;
    private bool boolMovimentacao = false;

    private int statusBurst = 0;
    public float danoBurst = 3f;

    private bool boolDefesa = false;

    void Start()
    {
        //player = FindObjectOfType<PlayerMovement>().transform;
        agent = GetComponent<NavMeshAgent>();
        vidaAtual = vida;
        _barraDeVida.AlterarBarraDeVida(vidaAtual, vida);

        stateInimigo = new Idle(gameObject, agent, player, municao, alcanceMaxArma, alcanceMinArma, cooldownTiro, velocidadeAndar); 
    }

    void Update()
    {
        stateInimigo = stateInimigo.Process();
    }

    public void TomarDano(float dano, string tipoDoDano)
    {
        if (vidaAtual == vida)
        {
            stateInimigo.ativarChase = true;
        }

        switch (tipoDoDano)
        {
            case "Critico":

                EfeitoCritico(dano);
                break;

            case "Defesa":

                EfeitoDefesa(dano);
                break;

            case "Movimentacao":

                EfeitoMovimentacao(dano);
                break;

            case "Corrosao":

                EfeitoCorrosao(dano);
                break;

            case "Burst":

                EfeitoBurst(dano);
                break;

            case "Default":                
                vidaAtual -= dano;
                _barraDeVida.AlterarBarraDeVida(vidaAtual, vida);
                break;
        }      

        if (vidaAtual <= 0)
        {
            DroparItem();
            Destroy(gameObject);
        }
    }



    public bool Atirar()
    {
        RaycastHit hit;

        if (!inimigoExplosivo && Physics.Raycast(pontaArma.position, transform.TransformDirection(Vector3.forward), out hit, alcanceMaxArma + 3))
        {

            if (hit.transform.tag == "Player")
            {
                tiro = objectPool.GetPooledObject().GetComponent<ProjetilInimigo>();
                tiro.InstanciaProjetil(danoTiro, pontaArma, velProjetil);
                tiro.gameObject.SetActive(true);

                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (inimigoExplosivo && Physics.CheckSphere(transform.position, alcanceMinArma, playerMask))
            {
                player.GetComponentInParent<VidaPlayer>().TomarDano(danoTiro);
                Debug.Log("Dano Completo");
                Destroy(gameObject);
            }
            else if (inimigoExplosivo && !Physics.CheckSphere(transform.position, alcanceMinArma, playerMask) && Physics.CheckSphere(transform.position, alcanceMaxArma, playerMask))
            {
                player.GetComponentInParent<VidaPlayer>().TomarDano(danoTiro / 2);
                Debug.Log("Metade do Dano");
                Destroy(gameObject);
            }
            else if (inimigoExplosivo && !Physics.CheckSphere(transform.position, alcanceMinArma, playerMask) && !Physics.CheckSphere(transform.position, alcanceMaxArma, playerMask))
            {
                Debug.Log("Nenhum Dano");
                Destroy(gameObject);
            }

            return true;
        }
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

    private void DroparItem()
    {
        Instantiate(dropPrefab, transform.position, transform.rotation);
    }


    #region Efeitos de Dano

    public void EfeitoCritico(float dano)
    {
        //Ideia: Ter um script que mantem todas as infos sobre os status do player, como a chance de critico por exemplo, e ai usar aqui :)

        int aux = Random.Range(1, 11);

        if(aux == 1)
        {
            dano *= 1.5f;
            Debug.Log("Critei" + dano);
        }

        vidaAtual -= dano;
        _barraDeVida.AlterarBarraDeVida(vidaAtual, vida);
    }

    public void EfeitoDefesa(float dano)
    {
        StatusDefesa();

        vidaAtual -= dano;
        _barraDeVida.AlterarBarraDeVida(vidaAtual, vida);

    }

    public void EfeitoCorrosao(float dano)
    {
        StatusCorrosao();

        vidaAtual -= dano;
        _barraDeVida.AlterarBarraDeVida(vidaAtual, vida);
    }

    public void EfeitoMovimentacao(float dano)
    {
        StatusMovimentacao();

        vidaAtual -= dano;
        _barraDeVida.AlterarBarraDeVida(vidaAtual, vida);
    }

    public void EfeitoBurst(float dano)
    {
        StatusBurst();

        vidaAtual -= dano;
        _barraDeVida.AlterarBarraDeVida(vidaAtual, vida);
    }


    private void StatusCorrosao()
    {
        if (!boolCorrosao)
        {
            statusCorrosao++;

            if (statusCorrosao >= 5)
            {
                StartCoroutine(CoroutineCorrosao());
            }

        }
    }

    private IEnumerator CoroutineCorrosao()
    {
        boolCorrosao = true;

        vidaAtual -= danoCorrosao;
        _barraDeVida.AlterarBarraDeVida(vidaAtual, vida);

        yield return new WaitForSeconds(1f);

        vidaAtual -= danoCorrosao;
        _barraDeVida.AlterarBarraDeVida(vidaAtual, vida);

        yield return new WaitForSeconds(1f);

        vidaAtual -= danoCorrosao;
        _barraDeVida.AlterarBarraDeVida(vidaAtual, vida);

        yield return new WaitForSeconds(1f);

        vidaAtual -= danoCorrosao;
        _barraDeVida.AlterarBarraDeVida(vidaAtual, vida);

        yield return new WaitForSeconds(1f);

        vidaAtual -= danoCorrosao;
        _barraDeVida.AlterarBarraDeVida(vidaAtual, vida);

        yield return new WaitForSeconds(1f);

        statusCorrosao = 0;

        boolCorrosao = false;
    }

    private void StatusMovimentacao()
    {
        if(!boolMovimentacao)
        {
            statusMovimentacao++;

            if(statusMovimentacao >= 3)
            {
                StartCoroutine(CoroutineMovimentacao());
            }
        }
    }

    private IEnumerator CoroutineMovimentacao()
    {
        boolMovimentacao = true;

        agent.speed = 2.5f;

        yield return new WaitForSeconds(5f);

        agent.speed = 3.5f;

        boolMovimentacao = false;
    }

    private void StatusBurst()
    {
        statusBurst++;

        if(statusBurst >= 5)
        {
            vidaAtual -= danoBurst;
            _barraDeVida.AlterarBarraDeVida(vidaAtual, vida);
            statusBurst = 0;
        }
    }

    private void StatusDefesa()
    {
        if(!boolDefesa)
        {
            StartCoroutine(CoroutineDefesa());
        }
    }

    private IEnumerator CoroutineDefesa()
    {
        boolDefesa = true;

        float aux = danoTiro;

        danoTiro = danoTiroReduzido;

        yield return new WaitForSeconds(5f);

        danoTiro = aux;
        boolDefesa = false;
    }

    #endregion
}
