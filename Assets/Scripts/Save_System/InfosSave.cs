using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class InfosSave
{
    //Informa��es sobre os itens no invent�rio
    public int[] quantidadeItem;

    //Informa��es sobre os buffs permanentes
    public int[] levelBuffPermanente;
    public bool[] buffDesbloqueado;

    //Informa��es sobre configura��o de volume
    public float masterVolume;
    public float musicVolume;
    public float ambienceVolume;
    public float sfxVolume;

    //Informa��es sobre configura��o da sensibilidade do mouse
    public float sensiOlhando;
    public float sensiMirando;

    //Informa��es sobre a narrativa

    public InfosSave()
    {
        //Setando valores que ser�o chamados quando o jogador iniciar um Novo Jogo

        //Setando valores sobre os itens no invent�rio
        quantidadeItem = new int[8];
        for (int i = 0; i < quantidadeItem.Length; i++)
        {
            quantidadeItem[i] = 0;
        }

        //Setando valores sobre os buffs permanentes
        levelBuffPermanente = new int[7];
        buffDesbloqueado = new bool[levelBuffPermanente.Length];
        for (int i = 0; i < levelBuffPermanente.Length; i++)
        {
            levelBuffPermanente[i] = 0;
            if(i == 0)
            {
                buffDesbloqueado[i] = true;
            }
            else
            {
                buffDesbloqueado[i] = false;
            }
        }

        //Setando valores sobre configura��o de volume
        masterVolume = .8f;
        musicVolume = .8f;
        ambienceVolume = .8f;
        sfxVolume = .8f;

        //Setando valores sobre a sensibilidade do mouse
        sensiOlhando = .5f;
        sensiMirando = .5f;
    }
}
