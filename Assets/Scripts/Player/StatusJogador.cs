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


    [Header("Acerto crítico")]
    public float chanceDeAcertoCriticoBase = 0;
    public float chanceDeAcertoCriticoAtual = 0;

    public float danoDoAcertoCritico = 1.5f;

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


    private void Start()
    {
        Reset();
    }

    private void Reset()
    {
        chanceDeAcertoCriticoBase = 0f;
        danoDoAcertoCritico = 1.5f;

        quantidadeDeDash = 1;
        quantidadeDeDashTotal = 1;
        duracaoBuffVelocidade = 3.5f;
        duracaoUltimateMovimentacao = 12f;
        misc1movimentacao = false;
    }


    #region BuffsMovimentação

    public void BuffMovimentacaoDash()
    {
        quantidadeDeDashTotal++;
        quantidadeDeDash = quantidadeDeDashTotal;
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

}
