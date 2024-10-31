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
    [SerializeField] private float vida;

    private float vidaAtual;
    private float vidaAtualizada;

    private StatusJogador statusJogador;

    public bool escudoAtivo;

    // Start is called before the first frame update
    private void Start()
    {
        vidaAtual = vida;
        AlterarBarraDeVida(vidaAtual, vida);
        vidaUI.text = vidaAtual.ToString() + " / " + vida.ToString();

        escudoAtivo = false;

        statusJogador = FindObjectOfType<StatusJogador>();
    }

    private void Update()
    {
        barraDeVida.value = Mathf.MoveTowards(barraDeVida.value, vidaAtualizada, velAnim * Time.deltaTime);

        //Se o jogador tiver a habilidade de ganhar mais dano com escudo ativo
        if(statusJogador.misc1Defesa)
        {
            if(escudoAtivo)
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

        if(escudoAtivo)
        {
            escudoAtivo = false;
            return;
        }

        vidaAtual -= dano;
        AlterarBarraDeVida(vidaAtual, vida);
        vidaUI.text = vidaAtual.ToString() + " / " + vida.ToString();

        //Buff misc da velocidade
        ConferindoBuffMiscMovimentacao();

        //Buff de escudo com vida baixa;
        ConferindoBuffMiscDefesa();

        if (vidaAtual <= 0)
        {
            EventBus.Instance.GameOver();
        }
    }

    public void AlterarBarraDeVida(float vidaAtual, float VidaMaxima)
    {
        vidaAtualizada = vidaAtual / VidaMaxima;
    }

    private void ConferindoBuffMiscMovimentacao()
    {
        if (vidaAtual <= vida / 4)
        {
            if (statusJogador.misc2movimentacao)
            {
                statusJogador.BuffVelocidade();
            }
        }
    }

    private void ConferindoBuffMiscDefesa()
    {
        if (vidaAtual <= vida / 4)
        {
            if (statusJogador.misc2Defesa)
            {
                AtivarEscudo();
            }
        }
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

        if(escudoAtivo)
        {
            escudoAtivo = false;
        }
    }
}
