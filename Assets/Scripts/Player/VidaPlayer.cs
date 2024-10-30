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

    // Start is called before the first frame update
    private void Start()
    {
        vidaAtual = vida;
        AlterarBarraDeVida(vidaAtual, vida);
        vidaUI.text = vidaAtual.ToString() + " / " + vida.ToString();

        statusJogador = FindObjectOfType<StatusJogador>();
    }

    private void Update()
    {
        barraDeVida.value = Mathf.MoveTowards(barraDeVida.value, vidaAtualizada, velAnim * Time.deltaTime);
    }

    public void TomarDano(float dano)
    {
        vidaAtual -= dano;
        AlterarBarraDeVida(vidaAtual, vida);
        vidaUI.text = vidaAtual.ToString() + " / " + vida.ToString();

        //Buff misc da velocidade
        ConferindoBuffMiscMovimentacao();

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
}
