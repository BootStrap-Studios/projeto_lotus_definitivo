using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.AI;
using static Cinemachine.CinemachineCore;
using static StateInimigos;
using static StateInimigoSniper;
using static StateInimigoExplosivo;
using static StateInimigoTorreta;

public class Inimigo : MonoBehaviour
{
    [Header("Outros")]
    [SerializeField] private Transform player;
    public GameObject olhosRobo;
    [SerializeField] private GameObject pontaArma;
    public Transform posPontaArma;
    public Transform peInimigo;
    [SerializeField] private ObjectPool objectPool;
    [SerializeField] private GameObject[] dropPrefab;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private ColliderArea colisaoArea;
    
    public Animator animator;
    public LayerMask playerMask, inimigoMask;
    private ProjetilInimigo tiro;   
    public SpawnInimigos spawnInimigos;

    public StateInimigos stateInimigoSimples;
    public StateInimigoSniper stateInimigoSniper;
    public StateInimigoExplosivo stateInimigoExplosivo;
    public StateInimigoTorreta stateInimigoTorreta;

    [Header("Configs da Arma")]
    [SerializeField] private int municao;
    [SerializeField] private float danoTiro;
    [SerializeField] private float danoTiroReduzido;
    [SerializeField] private float alcanceMaxArma;
    [SerializeField] private float alcanceMinArma;
    [SerializeField] private float cooldownTiro;
    [SerializeField] private float velProjetil;

    [Header("Configs do Inimigo")]
    public float velocidadeAndar;
    public GameObject objInimigo;
    [SerializeField] private GameObject cabecaTorreta;
    public bool inimigoExplosivo;
    public bool inimigoSniper;
    public bool inimigoSimples;
    public bool inimigoTorreta;
    private bool dropando;
    private bool explodindo;

    [Header("UI_Inimigos")]
    [SerializeField] private BarraDeVida _barraDeVida;
    [SerializeField] private float vida; 
    private float vidaAtual;


    [Header("Status do Inimigo")]
    private int statusCorrosao = 0;
    private bool boolCorrosao = false; 

    private int statusMovimentacao = 0;
    private bool boolMovimentacao = false;

    private int statusBurst = 0;

    private bool boolDefesa = false;

    private StatusJogador statusJogador;

    private float danoDaArma;

    public bool vulneravel = false;


    private void OnEnable()
    {
        EventBus.Instance.onAtivaInimigos += AtivarRobos;
    }

    private void OnDisable()
    {
        EventBus.Instance.onAtivaInimigos -= AtivarRobos;
    }

    void Start()
    {
        player = FindObjectOfType<VidaPlayer>().GetComponent<Transform>();
        objectPool = FindObjectOfType<ObjectPool>();
        statusJogador = FindObjectOfType<StatusJogador>();

        vidaAtual = vida;
        _barraDeVida.AlterarBarraDeVida(vidaAtual, vida);

        vulneravel = false;

        if (inimigoSimples)
        {
            stateInimigoSimples = new SimplesDesativado(gameObject, this, agent, player, municao, alcanceMaxArma, alcanceMinArma, cooldownTiro, animator);
        }
        else if (inimigoSniper)
        {
            stateInimigoSniper = new SniperDesativado(gameObject, this, agent, player, municao, alcanceMaxArma, alcanceMinArma, cooldownTiro, animator);
        }
        else if (inimigoExplosivo)
        {
            stateInimigoExplosivo = new ExplosivoDesativado(gameObject, this, agent, player, alcanceMaxArma, alcanceMinArma, cooldownTiro, animator);
        }
        else if (inimigoTorreta)
        { 
            stateInimigoTorreta = new TorretaDesativada(cabecaTorreta, this, agent, player, alcanceMaxArma, alcanceMinArma, cooldownTiro, municao);
        }

        EventBus.Instance.AtivaInimigos(true);
    }

    void Update()
    {
        _barraDeVida.AtualizaStatus(0, 1, "vulneravel", vulneravel);

        agent.speed = velocidadeAndar;

        if (inimigoSimples)
        {
            pontaArma.transform.position = posPontaArma.position;

            Vector3 lookPos = player.transform.position - pontaArma.transform.position;
            pontaArma.transform.rotation = Quaternion.LookRotation(new Vector3(lookPos.x, player.transform.position.y - 1.08f, lookPos.z));

            stateInimigoSimples = stateInimigoSimples.Process();
        }
        else if (inimigoSniper)
        {
            pontaArma.transform.position = posPontaArma.position;

            Vector3 lookPos = player.transform.position - pontaArma.transform.position;
            pontaArma.transform.rotation = Quaternion.LookRotation(new Vector3(lookPos.x, player.transform.position.y - 1.08f, lookPos.z));

            stateInimigoSniper = stateInimigoSniper.Process();
        }
        else if (inimigoExplosivo)
        {
            stateInimigoExplosivo = stateInimigoExplosivo.Process();
        }
        else if (inimigoTorreta)
        {
            pontaArma.transform.position = posPontaArma.position;

            Vector3 lookPos = player.transform.position - pontaArma.transform.position;
            pontaArma.transform.rotation = Quaternion.LookRotation(new Vector3(lookPos.x, player.transform.position.y - 1.78f, lookPos.z));

            Debug.Log(new Vector3(lookPos.x, player.transform.position.y - 1.78f, lookPos.z));

            stateInimigoTorreta = stateInimigoTorreta.Process();
        }
    }

    private void AtivarRobos(bool estado)
    {
        if (inimigoSimples) stateInimigoSimples.ativarRobo = estado;
        else if (inimigoSniper) stateInimigoSniper.ativarRobo = estado;
        else if (inimigoExplosivo) stateInimigoExplosivo.ativarRobo = estado;
        else if (inimigoTorreta) stateInimigoTorreta.ativarRobo = estado;
    }

    public void TomarDano(string tipoDeArma, string tipoDoDano)
    {
        if (inimigoSimples)
        {
            if (vidaAtual == vida && !stateInimigoSimples.ativarRobo)
            {
                stateInimigoSimples.ativarRobo = true;
                EventBus.Instance.AtivaInimigos(true);
                gameObject.transform.LookAt(player);
            }
            gameObject.transform.LookAt(player);
        }
        else if (inimigoSniper)
        {
            if (vidaAtual == vida && !stateInimigoSniper.ativarRobo)
            {
                stateInimigoSniper.ativarRobo = true;
                EventBus.Instance.AtivaInimigos(true);
                gameObject.transform.LookAt(player);
            }
            gameObject.transform.LookAt(player);
        }
        else if (inimigoExplosivo)
        {
            if (vidaAtual == vida && !stateInimigoExplosivo.ativarRobo)
            {
                stateInimigoExplosivo.ativarRobo = true;
                EventBus.Instance.AtivaInimigos(true);
                gameObject.transform.LookAt(player);
                agent.SetDestination(player.transform.position);
            }
            gameObject.transform.LookAt(player);
        }

        else if (inimigoTorreta)
        {
            if (vidaAtual == vida && !stateInimigoTorreta.ativarRobo)
            {
                stateInimigoTorreta.ativarRobo = true;
                EventBus.Instance.AtivaInimigos(true);
            }
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
            EventBus.Instance.Ultimate();
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
            Destroy(gameObject);
        }
    }

    private void MiscBurst2()
    {
        if(statusJogador.misc2Burst)
        {
            statusJogador.ReloadBurst();
            Destroy(gameObject);
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
            Destroy(gameObject);
        }
    }

    private IEnumerator MorrerAposExplosao()
    {
        yield return new WaitForSeconds(.3f);

        Destroy(gameObject);
    }

    private void Misc2Critico()
    {
        Inimigo inimigoX = FindObjectOfType<Inimigo>();

        if(inimigoX != null)
        {
            inimigoX.vulneravel = true;
        }

        Destroy(gameObject);
    }


    #endregion


    public bool Atirar(bool cover)
    {
        RaycastHit hit;

        if (!inimigoTorreta && Physics.Raycast(pontaArma.transform.position, transform.TransformDirection(Vector3.forward), out hit, alcanceMaxArma + 3))
        {
            if (hit.transform.tag == "Player" || cover)
            {
                tiro = objectPool.GetPooledObject().GetComponent<ProjetilInimigo>();
                tiro.InstanciaProjetil(danoTiro, pontaArma.transform, velProjetil);
                tiro.transform.LookAt(player.transform);
                tiro.gameObject.SetActive(true);

                if (inimigoSimples)
                {
                    AudioManager.instance.PlayOneShot(FMODEvents.instance.tiroInimigoBase, transform.position);
                }
                else if (inimigoSniper)
                {
                    AudioManager.instance.PlayOneShot(FMODEvents.instance.tiroInimigoSniper, transform.position);
                }

                return true;
            }
            else
            {
                return false;
            }
        }
        else if (inimigoTorreta)
        {
            tiro = objectPool.GetPooledObject().GetComponent<ProjetilInimigo>();
            tiro.InstanciaProjetil(danoTiro, pontaArma.transform, velProjetil);
            tiro.gameObject.SetActive(true);

            Debug.Log("PEW");
            AudioManager.instance.PlayOneShot(FMODEvents.instance.tiroInimigoTorreta, transform.position);

            return true;
        }
        else
        {
            return false;
        }
    }

    public void Explodir()
    {
        if (!explodindo)
        {
            Debug.Log("VAI EXPLODIIIR!!!");
            AudioManager.instance.PlayOneShot(FMODEvents.instance.avisoBomba, transform.position);
            StartCoroutine(Explodindo());
        }
    }

    private IEnumerator Explodindo()
    {
        explodindo = true;

        yield return new WaitForSeconds(1f);

        AudioManager.instance.PlayOneShot(FMODEvents.instance.bombaExplodiu, transform.position);

        if (Physics.CheckSphere(transform.position, alcanceMinArma, playerMask))
        {
            player.GetComponent<VidaPlayer>().TomarDano(danoTiro);
            //Debug.Log(gameObject.name + "aplicou dano completo");

            yield return new WaitForSeconds(1f);
            Destroy(gameObject);

        }
        else if (!Physics.CheckSphere(transform.position, alcanceMinArma, playerMask) && Physics.CheckSphere(transform.position, alcanceMaxArma, playerMask))
        {
            player.GetComponent<VidaPlayer>().TomarDano(danoTiro / 2);
            //Debug.Log(gameObject.name + "aplicou metade do dano");

            yield return new WaitForSeconds(1f);
            Destroy(gameObject);
        }
        else if (!Physics.CheckSphere(transform.position, alcanceMinArma, playerMask) && !Physics.CheckSphere(transform.position, alcanceMaxArma, playerMask))
        {
            //Debug.Log(gameObject.name + "não aplicou nenhum dano");

            yield return new WaitForSeconds(1f);
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        try
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, alcanceMaxArma);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, alcanceMinArma);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(pontaArma.transform.position, new Vector3(player.position.x, player.position.y, player.position.z));
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(olhosRobo.transform.position, new Vector3(player.position.x, player.position.y, player.position.z));
        }
        catch
        {

        }
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

        float posX = Random.Range(transform.position.x - 1, transform.position.x + 1);
        float posZ = Random.Range(transform.position.z - 1, transform.position.z + 1);

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
            _barraDeVida.AtualizaStatus(statusCorrosao, 3, "corrosao", true);


            if (statusCorrosao >= 3)
            {
                StartCoroutine(CoroutineCorrosao());
            }

        }
    }

    public void CorrosaoDireto()
    {
        if(!boolCorrosao)
        {
            statusCorrosao = 3;
            _barraDeVida.AtualizaStatus(statusCorrosao, 3, "corrosao", true);

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
        _barraDeVida.AtualizaStatus(statusCorrosao, 3, "corrosao", false);

        boolCorrosao = false;
    }

    private void StatusMovimentacao()
    {
        if(!boolMovimentacao)
        {
            statusMovimentacao++;
            _barraDeVida.AtualizaStatus(statusMovimentacao, 2, "movimentacao", true);

            if(statusMovimentacao >= 2)
            {
                StartCoroutine(CoroutineMovimentacao());
            }
        }
    }

    private IEnumerator CoroutineMovimentacao()
    {
        float velAux = velocidadeAndar;
        boolMovimentacao = true;

        velocidadeAndar = velocidadeAndar / 2;

        yield return new WaitForSeconds(5f);

        velocidadeAndar = velAux;

        statusMovimentacao = 0;
        _barraDeVida.AtualizaStatus(statusMovimentacao, 3, "movimentacao", false);
        boolMovimentacao = false;
    }

    private void StatusBurst()
    {
        statusBurst++;
        _barraDeVida.AtualizaStatus(statusBurst, 3, "burst", true);

        if(statusBurst >= 3)
        {
            vidaAtual -= statusJogador.danoBurst;
            AudioManager.instance.PlayOneShot(FMODEvents.instance.burstProc, transform.position);
            _barraDeVida.AlterarBarraDeVida(vidaAtual, vida);
            statusBurst = 0;
            _barraDeVida.AtualizaStatus(statusBurst, 3, "burst", false);
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
