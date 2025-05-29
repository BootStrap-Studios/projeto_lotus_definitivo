using FMODUnity;
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

    [SerializeField] private GameObject escudoVFX;

    private StudioEventEmitter escudoEmitter;

    // Start is called before the first frame update
    private void Start()
    {
        vidaAtual = vidaMaxima;
        AlterarBarraDeVida(vidaAtual, vidaMaxima);

        //escudoEmitter = AudioManager.instance.InitializeEventEmitter(FMODEvents.instance.escudoAtivo, gameObject);

        statusJogador = FindObjectOfType<StatusJogador>();
    }

    private void Update()
    {
        barraDeVida.value = Mathf.MoveTowards(barraDeVida.value, vidaAtualizada, velAnim * Time.deltaTime);

        if(statusJogador.escudoAtivo)
        {
            escudoVFX.SetActive(true);
            //escudoEmitter.Play();
        } else
        {
            escudoVFX.SetActive(false);
            //escudoEmitter.Stop();
            
        }

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
            AudioManager.instance.PlayOneShot(FMODEvents.instance.escudoQuebra, transform.position);
            return;
        }

        vidaAtual -= dano;
        AlterarBarraDeVida(vidaAtual, vidaMaxima);

        //Buff misc da velocidade
        ConferindoBuffMiscMovimentacao();

        //Buff de escudo com vida baixa;
        ConferindoBuffMiscDefesa();

        if (vidaAtual <= 0)
        {
            EventBus.Instance.SalvarJogo();
            SaveSystemManager.instance.SalvarJogo();
            EventBus.Instance.GameOver();
            vidaBaixa = false;
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
    }

    public void AlterarBarraDeVida(float vidaAtual, float vidaMaxima)
    {
        vidaAtualizada = vidaAtual / vidaMaxima;
        vidaUI.text = vidaAtual.ToString() + " / " + vidaMaxima.ToString();

        if (vidaAtual <= vidaMaxima / 4 && !vidaBaixa)
        {
            vidaBaixa = true;
            StartCoroutine(CO_VidaPiscandoFadeOut());
        }
        else
        {
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

        vidaIMG.color = new Color(vidaIMG.color.r, vidaIMG.color.g, vidaIMG.color.b, 1);

        if (vidaBaixa)
        {
            StartCoroutine(CO_VidaPiscandoFadeOut());
        }
    }

    private IEnumerator CO_VidaPiscandoFadeOut()
    {
        while (vidaIMG.color.a > 0)
        {
            vidaIMG.color = new Color(vidaIMG.color.r, vidaIMG.color.g, vidaIMG.color.b, vidaIMG.color.a - (Time.deltaTime / 0.3f));
            yield return null;
        }

        if (vidaBaixa)
        {
            vidaIMG.color = new Color(vidaIMG.color.r, vidaIMG.color.g, vidaIMG.color.b, 0);
            StartCoroutine(CO_VidaPiscandoFadeIn());
        }
        else
        {
            vidaIMG.color = new Color(vidaIMG.color.r, vidaIMG.color.g, vidaIMG.color.b, 1);
        }
    }

    #endregion
}
