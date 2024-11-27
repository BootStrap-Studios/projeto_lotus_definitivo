using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VidaPlayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI vidaUI;
    [SerializeField] private Slider barraDeVida;
    [SerializeField] private float velAnim;
    [SerializeField] public float vidaMaxima;
    [SerializeField] private Image vidaIMG;

    public float vidaAtual;
    private float vidaAtualizada;
    private bool vidaBaixa;

    private StatusJogador statusJogador;

    

    // Start is called before the first frame update
    private void Start()
    {
        vidaAtual = vidaMaxima;
        AlterarBarraDeVida(vidaAtual, vidaMaxima);
        vidaUI.text = vidaAtual.ToString() + " / " + vidaMaxima.ToString();

        

        statusJogador = FindObjectOfType<StatusJogador>();
    }

    private void Update()
    {
        barraDeVida.value = Mathf.MoveTowards(barraDeVida.value, vidaAtualizada, velAnim * Time.deltaTime);

        //Se o jogador tiver a habilidade de ganhar mais dano com escudo ativo
        if(statusJogador.misc1Defesa)
        {
            if(statusJogador.escudoAtivo)
            {
                statusJogador.danoAtualPistola += 2;
                statusJogador.danoAtualShotgun += 2;
                statusJogador.danoAtualShuriken += 2;
            } else
            {
                statusJogador.danoAtualPistola = statusJogador.danoBasePistola;
                statusJogador.danoAtualShotgun = statusJogador.danoBaseShotgun;
                statusJogador.danoAtualShuriken = statusJogador.danoBaseShuriken;
            }
        }
    }

    public void TomarDano(float dano)
    {
        if(statusJogador.ultimateDefesa)
        {
            //Ligar o vfx do escudo e é nozes
            return;
        }

        if(statusJogador.escudoAtivo)
        {
            statusJogador.escudoAtivo = false;
            return;
        }

        vidaAtual -= dano;
        AlterarBarraDeVida(vidaAtual, vidaMaxima);
        vidaUI.text = vidaAtual.ToString() + " / " + vidaMaxima.ToString();

        //Buff misc da velocidade
        ConferindoBuffMiscMovimentacao();

        //Buff de escudo com vida baixa;
        ConferindoBuffMiscDefesa();

        if (vidaAtual <= 0)
        {
            EventBus.Instance.GameOver();
        }
    }

    public void CurarVida(float quantaCura)
    {
        vidaAtual += quantaCura;

        if(vidaAtual > vidaMaxima)
        {
            vidaAtual = vidaMaxima;
        }

        AlterarBarraDeVida(vidaAtual, vidaMaxima);
        vidaUI.text = vidaAtual.ToString() + " / " + vidaMaxima.ToString();
    }

    public void AlterarBarraDeVida(float vidaAtual, float VidaMaxima)
    {
        vidaAtualizada = vidaAtual / VidaMaxima;

        if (vidaAtual <= VidaMaxima / 4 && !vidaBaixa)
        {
            vidaBaixa = true;
            StartCoroutine(CO_VidaPiscandoFadeOut());
        }
        else
        {
            vidaIMG.color = new Color(vidaIMG.color.r, vidaIMG.color.g, vidaIMG.color.b, 1);
            vidaBaixa = false;
        }
    }

    private void ConferindoBuffMiscMovimentacao()
    {
        if (vidaAtual <= vidaMaxima / 4)
        {
            if (statusJogador.misc2movimentacao)
            {
                statusJogador.BuffVelocidade();
            }
        }
    }

    private void ConferindoBuffMiscDefesa()
    {
        if (vidaAtual <= vidaMaxima / 4)
        {
            if (statusJogador.misc2Defesa)
            {
                statusJogador.AtivarEscudo();
            }
        }
    }

    

    #region Animação da barra piscando

    private IEnumerator CO_VidaPiscandoFadeIn()
    {       
        while (vidaIMG.color.a < 1)
        {
            vidaIMG.color = new Color(vidaIMG.color.r, vidaIMG.color.g, vidaIMG.color.b, vidaIMG.color.a + (Time.deltaTime / 0.3f));
            yield return null;
        }

        yield return new WaitForSeconds(.3f);
        if (!vidaBaixa) StartCoroutine(CO_VidaPiscandoFadeOut());
    }

    private IEnumerator CO_VidaPiscandoFadeOut()
    {
        while (vidaIMG.color.a > 0)
        {
            vidaIMG.color = new Color(vidaIMG.color.r, vidaIMG.color.g, vidaIMG.color.b, vidaIMG.color.a - (Time.deltaTime / 0.3f));
            yield return null;
        }

        if (!vidaBaixa) StartCoroutine(CO_VidaPiscandoFadeIn());
    }

    #endregion
}
