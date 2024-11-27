using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StatusJogador : MonoBehaviour
{
    [Header("Cura por Sala")]
    public float curaPorSala;

    [Header("Dano base das armas")]
    public float danoBasePistola;
    public float danoBaseShotgun;
    public float danoBaseShuriken;


    [Header("Dano atual das armas (Após modificadores)")]
    public float danoAtualPistola;
    public float danoAtualShotgun;
    public float danoAtualShuriken;

    [Header("Armas")]
    [SerializeField] private Arma arma;
    [SerializeField] private Shotgun shotgun;
    [SerializeField] private Shuriken shuriken;
    [SerializeField] private AmmoSystem ammoSystem;
    [SerializeField] private LayerMask aimColliderMask;
    [SerializeField] private LayerMask layerInimigo;
    [SerializeField] private GameObject vfxTiroUltBurst;

    [Header("Armadilhas")]
    public float danoArmadilha1;


    [Header("Acerto crítico")]
    [SerializeField] private ColliderArea colliderAreaCritico;
    public float chanceDeAcertoCriticoBase = 0;
    public float chanceDeAcertoCriticoAtual = 0;

    public float danoDoAcertoCritico = 1.5f;

    public bool dashAcertoCritico;

    public bool misc2Critico;

    [Header("Movimentação")]
    public float velocidadeAndando;
    public float velocidadePulando;
    public float duracaoBuffVelocidade;
    public int quantidadeDeDashTotal;
    public int quantidadeDeDash;
    public float duracaoUltimateMovimentacao;

    //Essa variavel serve para a habilidade misc de movimentação, onde se um inimigo for derrotado ele da speed ao jogador
    public bool misc1movimentacao;

    //Essa variavel serve para saber se o jogador tem o buff que da velocidade caso chegue a 1/4 da vida;
    public bool misc2movimentacao;


    [Header("Burst")]

    public bool dashBurst;
    public float danoBurst;

    //Essa variavel é para aumentar o dano quando os inimigos sao derrotados
    public bool misc2Burst;

    //Essa variavel é para os inimigos explodirem
    public bool misc3Burst;

    [Header("Defesa")]
    public bool ultimateDefesa;
    public bool escudoAtivo;

    //Variavel para a misc de defesa, onde o jogador ganha mais dano se estiver com escudo
    public bool misc1Defesa;
    public bool misc2Defesa;

    [Header("Corrosao")]
    public float danoCorrosao;
    public int duracaoCorrosao;
    public float duracaoPoca;
    [SerializeField] private GameObject pocaCorrosao;
    [SerializeField] private GameObject peDoJogador;
    [SerializeField] private GameObject ultCorrosaoObj;

    [Header("ULT")]
    public bool tenhoULT;
    public string qualULT;
    public int statusULT;
    private int ultValorAtual;
    [SerializeField] private Slider barraULT;

    [Header("Dashes e Reload")]
    public string qualReload;
    public bool dashDefesaAtivo;
    public bool dashCorrosaoAtivo;

    [Header("Outros scripts")]
    [SerializeField] private VidaPlayer vidaPlayer;
    [SerializeField] private GameObject terminalBuffSala0;
    [SerializeField] private GameObject[] spawnInimigos;

    private void Start()
    {
        Reset();
    }

    private void Update()
    {
        barraULT.value = Mathf.MoveTowards(barraULT.value, ultValorAtual, 0.5f * Time.deltaTime);
    }

    private void OnEnable()
    {
        EventBus.Instance.onUltimate += StatusUlt;
    }

    private void OnDisable()
    {
        EventBus.Instance.onUltimate -= StatusUlt;
    }

    public void Reset()
    {
        danoAtualPistola = danoBasePistola;
        danoAtualShotgun = danoBaseShotgun;
        danoAtualShuriken = danoBaseShuriken;

        danoArmadilha1 = 5;

        qualReload = "Nenhum";
        arma.tipoDoBuff = "Nenhum";
        shotgun.tipoDoBuff = "Nenhum";
        shuriken.tipoDoBuff = "Nenhum";

        chanceDeAcertoCriticoBase = 0f;
        danoDoAcertoCritico = 1.5f;
        dashAcertoCritico = false;
        misc2Critico = false;

        quantidadeDeDash = 1;
        quantidadeDeDashTotal = 1;
        duracaoBuffVelocidade = 3.5f;
        duracaoUltimateMovimentacao = 12f;
        misc1movimentacao = false;

        dashBurst = false;
        danoBurst = 3f;
        misc2Burst = false;
        misc3Burst = false;

        danoCorrosao = 5f;
        duracaoCorrosao = 5;
        duracaoPoca = 5f;
        dashCorrosaoAtivo = false;

        ultimateDefesa = false;
        misc1Defesa = false;
        misc2Defesa = false;
        escudoAtivo = false;
        dashDefesaAtivo = false;

        tenhoULT = false;
        qualULT = "Nenhum";

        terminalBuffSala0.GetComponent<BoxCollider>().enabled = true;
        AtivarSpawns();

        vidaPlayer.vidaAtual = vidaPlayer.vidaMaxima;
        vidaPlayer.AlterarBarraDeVida(vidaPlayer.vidaAtual, vidaPlayer.vidaMaxima);

        barraULT.gameObject.SetActive(false);
        ResetULT();
    }


    #region BuffsMovimentação

    public void BuffDisparoMovimentacao()
    {
        arma.tipoDoBuff = "Movimentacao";
        shotgun.tipoDoBuff = "Movimentacao";
        shuriken.tipoDoBuff = "Movimentacao";
    }
    public void BuffDashMovimentacao()
    {
        quantidadeDeDashTotal++;
        quantidadeDeDash = quantidadeDeDashTotal;
    }

    public void BuffReloadMovimentacao()
    {
        qualReload = "Movimentacao";
    }

    //Destruir um inimigo da velocidade de movimento
    public void BuffMisc1Movimentacao()
    {
        misc1movimentacao = true;
    }

    //Player ganhar velocidade ao ficar com pouca vida
    public void BuffMisc2Movimentacao()
    {
        misc2movimentacao = true;
    }

    public void BuffMisc3Movimentacao()
    {
        AumentarDuracaoBuffVelocidade();
    }


    public void BuffVelocidade()
    {
        StartCoroutine(BuffVelocidadeCoroutine());
    }

    private IEnumerator BuffVelocidadeCoroutine()
    {
        velocidadeAndando = 15f;
        velocidadePulando = 11.8f;

        yield return new WaitForSeconds(duracaoBuffVelocidade);

        velocidadeAndando = 8f;
        velocidadePulando = 4.8f;
    }

    public void AumentarDuracaoBuffVelocidade()
    {
        duracaoBuffVelocidade = 5.5f;
    }

    //Depois no codigo que for ativar a ultimate, chamar essa funcao + a municao infinita no ammo system
    public void UltimateMovimentacao()
    {
        Debug.Log("Ult movimentacao");
        StartCoroutine(UltimateMovimentacaoCoroutine());
        ammoSystem.MunicaoInfinita();

    }

    private IEnumerator UltimateMovimentacaoCoroutine()
    {
        float aux = duracaoBuffVelocidade;
        duracaoBuffVelocidade = duracaoUltimateMovimentacao;

        BuffVelocidade();

        yield return new WaitForSeconds(duracaoUltimateMovimentacao);

        duracaoBuffVelocidade = aux;
    }



    #endregion

    #region  BuffsBurst

    public void ReloadBurst()
    {
        StartCoroutine(ReloadBurstCoroutine());
    }

    private IEnumerator ReloadBurstCoroutine()
    {
        danoAtualPistola += 2;
        danoAtualShotgun += 2;
        danoAtualShuriken += 2;

        yield return new WaitForSeconds(3f);

        danoAtualPistola = danoBasePistola;
        danoAtualShotgun = danoBaseShotgun;
        danoAtualShuriken = danoBaseShuriken;

    }



    public void BuffDisparoBurst()
    {
        arma.tipoDoBuff = "Burst";
        shotgun.tipoDoBuff = "Burst";
        shuriken.tipoDoBuff = "Burst";
    }

    public void BuffDashBurst()
    {
        dashBurst = true;
    }

    public void BuffReloadBurst()
    {
        qualReload = "Burst";
    }

    public void BuffMisc1Burst()
    {
        danoBurst = 5f;
    }

    public void BuffMisc2Burst()
    {
        misc2Burst = true;
    }

    public void BuffMisc3Burst()
    {
        misc3Burst = true;
    }

    private void UltimateBurst()
    {
        Debug.Log("Ult Burst");

        RaycastHit hit;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        float distancia;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~layerInimigo))
        {
            distancia = hit.distance;
        }
        else
        {
            distancia = Mathf.Infinity;
        }

        RaycastHit[] hits;

        hits = Physics.RaycastAll(ray, distancia);

        foreach (RaycastHit hitao in hits)
        {
            if (hitao.collider.CompareTag("Inimigo"))
            {
                hitao.transform.GetComponent<Inimigo>().TomarDanoDireto(1000f);
                Instantiate(vfxTiroUltBurst, hit.point, Quaternion.identity);



            }
            else
            {
                Instantiate(vfxTiroUltBurst, hit.point, Quaternion.identity);


            }
        }

    }

    #endregion

    #region BuffsDefesa

    public void UltimateDefesa()
    {
        Debug.Log("Ult defesa");
        StartCoroutine(UltimateDefesaCoroutine());
    }

    private IEnumerator UltimateDefesaCoroutine()
    {
        ultimateDefesa = true;

        yield return new WaitForSeconds(12f);

        ultimateDefesa = false;
    }



    public void AtivarEscudo()
    {
        if (!escudoAtivo)
        {
            StartCoroutine(AtivarEscudoCoroutine());
        }

    }

    private IEnumerator AtivarEscudoCoroutine()
    {
        escudoAtivo = true;

        yield return new WaitForSeconds(3f);

        if (escudoAtivo)
        {
            escudoAtivo = false;
        }
    }
    public void BuffTiroDefesa()
    {
        arma.tipoDoBuff = "Defesa";
        shotgun.tipoDoBuff = "Defesa";
        shuriken.tipoDoBuff = "Defesa";
    }

    public void BuffDashDefesa()
    {
        dashDefesaAtivo = true;
    }

    public void BuffReloadDefesa()
    {
        qualReload = "Defesa";
    }

    public void BuffMisc2Defesa()
    {
        misc2Defesa = true;
    }

    public void ReceberMenosDanoArmadilha()
    {
        danoArmadilha1 = 3f;
    }

    #endregion

    #region BuffsCorrosao

    public void BuffTiroCorrosao()
    {
        arma.tipoDoBuff = "Corrosao";
        shotgun.tipoDoBuff = "Corrosao";
        shuriken.tipoDoBuff = "Corrosao";
    }
    public void SpawnarPocaCorrosao()
    {
        Instantiate(pocaCorrosao, peDoJogador.transform.position, Quaternion.identity);
    }

    public void BuffDashCorrosao()
    {
        dashCorrosaoAtivo = true;
    }

    public void BuffReloadCorrosao()
    {
        qualReload = "Corrosao";
    }

    public void BuffMisc1Corrosao()
    {
        danoCorrosao = 10f;
    }

    public void BuffMisc2Corrosao()
    {
        duracaoCorrosao = 7;
    }

    public void BuffMisc3Corrosao()
    {
        duracaoPoca = 7;
    }

    private void UltimateCorrosao()
    {
        Debug.Log("Ult movimentacao");

        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderMask))
        {
            Instantiate(ultCorrosaoObj, raycastHit.point, Quaternion.identity);
        }
    }

    #endregion

    #region BuffsCritico

    public void BuffDisparoCritico()
    {
        arma.tipoDoBuff = "Critico";
        shotgun.tipoDoBuff = "Critico";
        shuriken.tipoDoBuff = "Critico";

        chanceDeAcertoCriticoAtual = 3;
    }

    public void BuffDashCritico()
    {
        dashAcertoCritico = true;
    }

    public void BuffReloadCritico()
    {
        qualReload = "Critico";
    }

    public void BuffMisc1Critico()
    {
        danoDoAcertoCritico = 1.8f;
    }

    public void BuffMisc2Critico()
    {
        misc2Critico = true;
    }

    private void UltimateCritico()
    {
        Debug.Log("Ult Critico");
        StartCoroutine(UltimateCriticoCoroutine());
    }

    private IEnumerator UltimateCriticoCoroutine()
    {
        //DEVERIA TER ALGUM SOM OU EFEITO NA TELA
        float aux = chanceDeAcertoCriticoAtual;
        chanceDeAcertoCriticoAtual = 10f;

        yield return new WaitForSeconds(12f);

        chanceDeAcertoCriticoAtual = aux;
    }

    #endregion


    //quando jogador selecionar uma ult chamar essa função
    public void DesbloqueiaUlt(string _qualUlt)
    {
        tenhoULT = true;
        qualULT = _qualUlt;
        barraULT.gameObject.SetActive(true);
    }

    public void Ultando()
    {
        //switch case com todas as ult que ira chamar a função da ult no StatusJogador
        Debug.Log("Ultando");
        switch (qualULT)
        {
            case "Movimentacao":
                UltimateMovimentacao();

                break;

            case "Defesa":
                UltimateDefesa();

                break;

            case "Critico":
                UltimateCritico();

                break;

            case "Burst":
                UltimateBurst();

                break;

            case "Corrosao":
                UltimateCorrosao();

                break;
        }

        ResetULT();
    }

    private void StatusUlt()
    {
        if (tenhoULT)
        {
            statusULT++;
            ultValorAtual = statusULT / 10;
        }
    }

    public void ResetULT()
    {
        statusULT = 0;
        ultValorAtual = statusULT / 10;
    }

    public void DandoUlt(string ult)
    {
        tenhoULT = true;

        qualULT = ult;

        barraULT.gameObject.SetActive(true);
    }

    public void ReloadBuffs()
    {
        switch (qualReload)
        {
            case "Nenhum":
                break;

            case "Corrosao":
                SpawnarPocaCorrosao();
                break;

            case "Burst":
                ReloadBurst();
                break;

            case "Movimentacao":
                BuffVelocidade();
                break;

            case "Critico":
                Debug.Log("teste3");
                colliderAreaCritico.gameObject.SetActive(true);
                colliderAreaCritico.OqueFazer("vulneravel");
                break;

            case "Defesa":
                AtivarEscudo();
                break;
        }
    }

    public void AtualizaStatus()
    {
        danoAtualPistola = danoBasePistola;
        danoAtualShotgun = danoBaseShotgun;
        danoAtualShuriken = danoBaseShuriken;

        vidaPlayer.vidaAtual = vidaPlayer.vidaMaxima;
    }

    private void AtivarSpawns()
    {
        foreach (GameObject spawn in spawnInimigos)
        {
            spawn.GetComponent<BoxCollider>().enabled = true;

        }

        foreach (GameObject spawn in spawnInimigos)
        {
            spawn.GetComponent<SpawnInimigos>().terminalBuff.GetComponent<BoxCollider>().enabled = true; ;
        }  
    }
}
