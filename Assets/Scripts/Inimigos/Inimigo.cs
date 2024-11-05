using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
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
    [SerializeField] private GameObject[] dropPrefab;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private ColliderArea colisaoArea;
    public LayerMask playerMask, inimigoMask;
    private ProjetilInimigo tiro;   
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
    [SerializeField] private GameObject objInimigo;
    public bool inimigoExplosivo;
    public bool inimigoSniper;
    public bool inimigoNormal;
    public bool inimigoTorreta;
    private bool dropando;

    [Header("UI_Inimigos")]
    [SerializeField] private BarraDeVida _barraDeVida;
    [SerializeField] private float vida; 
    [SerializeField] private TextMeshProUGUI statusInimigo; 
    private float vidaAtual;


    [Header("Status")]
    private int statusCorrosao = 0;
    private bool boolCorrosao = false; 

    private int statusMovimentacao = 0;
    private bool boolMovimentacao = false;

    private int statusBurst = 0;

    private bool boolDefesa = false;

    private StatusJogador statusJogador;

    private float danoDaArma;

    public bool vulneravel;

    void Start()
    {
        player = FindObjectOfType<PlayerMovement>().GetComponent<Transform>();
        objectPool = FindObjectOfType<ObjectPool>();
        statusJogador = FindObjectOfType<StatusJogador>();

        vidaAtual = vida;
        _barraDeVida.AlterarBarraDeVida(vidaAtual, vida);

        vulneravel = false;

        stateInimigo = new Idle(gameObject, agent, player, municao, alcanceMaxArma, alcanceMinArma, cooldownTiro, velocidadeAndar); 
    }

    void Update()
    {
        stateInimigo = stateInimigo.Process();

        _barraDeVida.AtualizaStatus(1, 1, "vulneravel", vulneravel);
    }

    public void TomarDano(string tipoDeArma, string tipoDoDano)
    { 
        if (vidaAtual == vida)
        {
            stateInimigo.ativarChase = true;
        }

        switch(tipoDeArma)
        {
            case "Pistola":

                danoDaArma = statusJogador.danoAtualPistola;

                break;

            case "Shotgun":

                danoDaArma = statusJogador.danoAtualShotgun;

                break;

            case "Shuriken":

                danoDaArma = statusJogador.danoAtualShuriken;

                break;
        }


        switch (tipoDoDano)
        {
            case "Critico":

                
                    EfeitoCritico(danoDaArma);
              
                
                break;

            case "Defesa":

                if(vulneravel)
                {
                    EfeitoDefesa(danoDaArma *= statusJogador.danoDoAcertoCritico);
                    vulneravel = false;
                    Debug.Log("Critei");
                } else
                {
                    EfeitoDefesa(danoDaArma);
                }
                
                break;

            case "Movimentacao":

                if(vulneravel)
                {
                    EfeitoMovimentacao(danoDaArma *= statusJogador.danoDoAcertoCritico);
                    vulneravel = false;
                    Debug.Log("Critei");
                }
                else
                {
                    EfeitoMovimentacao(danoDaArma);
                }
                
                break;

            case "Corrosao":

                if(vulneravel)
                {
                    EfeitoCorrosao(danoDaArma *= statusJogador.danoDoAcertoCritico);
                    vulneravel = false;
                    Debug.Log("Critei");
                } else
                {
                    EfeitoCorrosao(danoDaArma);
                }
                
                break;

            case "Burst":

                if(vulneravel)
                {
                    EfeitoBurst(danoDaArma *= statusJogador.danoDoAcertoCritico);
                    vulneravel = false;
                    Debug.Log("Critei");

                } else
                {
                    EfeitoBurst(danoDaArma);
                }
                
                break;

            case "Nenhum":
                if(vulneravel)
                {
                    vidaAtual -= danoDaArma *= statusJogador.danoDoAcertoCritico;
                    _barraDeVida.AlterarBarraDeVida(vidaAtual, vida);
                    vulneravel = false; 
                } else
                {
                    vidaAtual -= danoDaArma;
                    _barraDeVida.AlterarBarraDeVida(vidaAtual, vida);
                }
                

                break;
        }

        ConferirMorte();
    }

    public void TomarDanoDireto(float dano)
    {
        vidaAtual -= dano;
        _barraDeVida.AlterarBarraDeVida(vidaAtual, vida);

        ConferirMorte();
    }

    private void ConferirMorte()
    {
        if (vidaAtual <= 0)
        {
            DroparItem();

            MiscMovimentacao();
            MiscBurst2();
            MiscBurst3();
            Misc2Critico();
        }
    }

    #region BuffsQueAfetamOInimigoAoMorrer

    private void MiscMovimentacao()
    {
        if (statusJogador.misc1movimentacao)
        {
            statusJogador.BuffVelocidade();
            Destroy(objInimigo);
        }
    }

    private void MiscBurst2()
    {
        if(statusJogador.misc2Burst)
        {
            statusJogador.ReloadBurst();
            Destroy(objInimigo);
        }
        
    }

    private void MiscBurst3()
    {
        if (statusJogador.misc3Burst)
        {
            colisaoArea.gameObject.SetActive(true);
            colisaoArea.OqueFazer("darDanoExplosao");
            StartCoroutine(MorrerAposExplosao());
        }
        else
        {
            Destroy(objInimigo);
        }
    }

    private IEnumerator MorrerAposExplosao()
    {
        yield return new WaitForSeconds(.3f);

        Destroy(objInimigo);
    }

    private void Misc2Critico()
    {
        Inimigo inimigoX = FindObjectOfType<Inimigo>();

        if(inimigoX != null)
        {
            inimigoX.vulneravel = true;
        }
    }
    

    #endregion

    public bool Atirar()
    {        
        RaycastHit hit;

        if (!inimigoExplosivo && Physics.Raycast(pontaArma.position, transform.TransformDirection(Vector3.forward), out hit, alcanceMaxArma + 3))
        {          
            if (hit.transform.tag == "Player" || inimigoTorreta)
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
        if (!dropando)
        {
            dropando = true;
        }
        else
        {
            return;
        }

        float posX = Random.Range(transform.position.x - 2, transform.position.x + 2);
        float posZ = Random.Range(transform.position.z - 2, transform.position.z + 2);

        Vector3 posDrop = new Vector3(posX, transform.position.y, posZ);

        Instantiate(dropPrefab[0], posDrop, transform.rotation);
        int itemExtra = Random.Range(1, 11);

        if(itemExtra <= 3)
        {
            float posX2 = Random.Range(transform.position.x - 2, transform.position.x + 2);
            float posZ2 = Random.Range(transform.position.z - 2, transform.position.z + 2);

            posDrop = new Vector3(posX2, transform.position.y, posZ2);

            int qualItemDropar = Random.Range(1, dropPrefab.Length);
            Instantiate(dropPrefab[qualItemDropar], posDrop, transform.rotation);
        }
    }


    #region Efeitos de Dano

    public void EfeitoCritico(float dano)
    {

        if(vulneravel)
        {
            dano *= statusJogador.danoDoAcertoCritico;
            vidaAtual -= dano;
            _barraDeVida.AlterarBarraDeVida(vidaAtual, vida);

        } else
        {
            int aux = Random.Range(1, 11);

            if (aux <= statusJogador.chanceDeAcertoCriticoAtual)
            {
                dano *= statusJogador.danoDoAcertoCritico;  
            }

            vidaAtual -= dano;
            _barraDeVida.AlterarBarraDeVida(vidaAtual, vida);
        }
        
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
            _barraDeVida.AtualizaStatus(statusCorrosao, 5, "corrosao", true);


            if (statusCorrosao >= 5)
            {
                StartCoroutine(CoroutineCorrosao());
            }

        }
    }

    public void CorrosaoDireto()
    {
        if(!boolCorrosao)
        {
            statusCorrosao = 5;
            _barraDeVida.AtualizaStatus(statusCorrosao, 5, "corrosao", true);

            StartCoroutine(CoroutineCorrosao());
        }
        
    }

    private IEnumerator CoroutineCorrosao()
    {
        boolCorrosao = true;

        int i = 0;

        while(i < statusJogador.duracaoCorrosao)
        {
            vidaAtual -= statusJogador.danoCorrosao;
            _barraDeVida.AlterarBarraDeVida(vidaAtual, vida);

            ConferirMorte();

            yield return new WaitForSeconds(1f);

            i++;
        }

        statusCorrosao = 0;
        _barraDeVida.AtualizaStatus(statusCorrosao, 5, "corrosao", false);

        boolCorrosao = false;
    }

    private void StatusMovimentacao()
    {
        if(!boolMovimentacao)
        {
            statusMovimentacao++;
            _barraDeVida.AtualizaStatus(statusMovimentacao, 3, "movimentacao", true);

            if(statusMovimentacao >= 3)
            {
                StartCoroutine(CoroutineMovimentacao());
            }
        }
    }

    private IEnumerator CoroutineMovimentacao()
    {
        float velAux = velocidadeAndar;
        boolMovimentacao = true;

        velocidadeAndar = 2.5f;

        yield return new WaitForSeconds(5f);

        velocidadeAndar = velAux;

        statusMovimentacao = 0;
        _barraDeVida.AtualizaStatus(statusMovimentacao, 3, "movimentacao", false);
        boolMovimentacao = false;
    }

    private void StatusBurst()
    {
        statusBurst++;
        _barraDeVida.AtualizaStatus(statusBurst, 5, "burst", true);

        if(statusBurst >= 5)
        {
            vidaAtual -= statusJogador.danoBurst;
            _barraDeVida.AlterarBarraDeVida(vidaAtual, vida);
            statusBurst = 0;
            _barraDeVida.AtualizaStatus(statusBurst, 5, "burst", false);
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

        _barraDeVida.AtualizaStatus(1, 1, "fraco", boolDefesa);

        float aux = danoTiro;

        danoTiro = danoTiroReduzido;

        yield return new WaitForSeconds(5f);

        danoTiro = aux;
        boolDefesa = false;

        _barraDeVida.AtualizaStatus(1, 1, "fraco", boolDefesa);
    }

    #endregion
}
