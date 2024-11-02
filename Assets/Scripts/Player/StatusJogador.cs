using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusJogador : MonoBehaviour
{
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

    [Header("Armadilhas")]
    public float danoArmadilha1;


    [Header("Acerto crítico")]
    public float chanceDeAcertoCriticoBase = 0;
    public float chanceDeAcertoCriticoAtual = 0;

    public float danoDoAcertoCritico = 1.5f;

    public bool dashAcertoCritico;

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

    [Header("ULT")]
    public bool tenhoULT;
    public string qualULT;

    [Header("Dashes e Reload")]
    public string qualReload;
    public bool dashDefesaAtivo;
    public bool dashCorrosaoAtivo;


    private void Start()
    {
        Reset();
    }

    private void Reset()
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

        quantidadeDeDash = 1;
        quantidadeDeDashTotal = 1;
        duracaoBuffVelocidade = 3.5f;
        duracaoUltimateMovimentacao = 12f;
        misc1movimentacao = false;

        dashBurst = false;
        danoBurst = 3f;
        misc2Burst = false;
        misc3Burst = false;

        danoCorrosao = 1f;
        duracaoCorrosao = 5;
        duracaoPoca = 5f;
        dashCorrosaoAtivo = false;

        ultimateDefesa = false;
        misc1Defesa = false;
        misc2Defesa = false;
        escudoAtivo = false;
        dashDefesaAtivo = false;

        tenhoULT = false;
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
        velocidadeAndando = 10;
        velocidadePulando = 6.8f;

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
        StartCoroutine(UltimateMovimentacaoCoroutine());
    }

    private IEnumerator UltimateMovimentacaoCoroutine()
    {
        float aux = duracaoBuffVelocidade;
        duracaoBuffVelocidade = duracaoUltimateMovimentacao;

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



    #endregion

    #region BuffsDefesa

    public void UltimateDefesa()
    {
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
        Instantiate(pocaCorrosao, peDoJogador.transform);
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
        danoCorrosao = 1.5f;
    }

    public void BuffMisc2Corrosao()
    {
        duracaoCorrosao = 7;
    }

    public void BuffMisc3Corrosao()
    {
        duracaoPoca = 7;
    }
    #endregion


    //quando jogador selecionar uma ult chamar essa função
    public void DesbloqueiaUlt(string _qualUlt)
    {
        tenhoULT = true;
        qualULT = _qualUlt;
    }

    public void Ultando()
    {
        //switch case com todas as ult que ira chamar a função da ult no StatusJogador

    }

    public void DesativandoBotoesBuffsUI()
    {
        BuffManager buff = FindObjectOfType<BuffManager>();
        buff.DesativandoTodosOsBotoes();
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

                break;

            case "Defesa":
                AtivarEscudo();
                break;
        }
    }
}
