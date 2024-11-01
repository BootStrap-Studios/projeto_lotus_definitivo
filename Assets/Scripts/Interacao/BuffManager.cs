using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuffManager : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject botoesArea;

    [SerializeField] private GameObject[] botoesDefesa;
    [SerializeField] private GameObject[] botoesCorrosao;
    [SerializeField] private GameObject[] botoesCritico;
    [SerializeField] private GameObject[] botoesMovimentacao;
    [SerializeField] private GameObject[] botoesBurst;

    private StatusJogador statusJogador;

    private void Start()
    {
        statusJogador = FindObjectOfType<StatusJogador>();
    }
    public void Interagir()
    {
        SorteandoQualArvore();
    }

    private void SorteandoQualArvore()
    {
        int aux = Random.Range(1, 5);

        switch(aux)
        {
            case 1:

                SorteandoQuaisBuffs(botoesDefesa);
                break;

            case 2:
                SorteandoQuaisBuffs(botoesCorrosao);
                break;

            case 3:
                SorteandoQuaisBuffs(botoesBurst);
                break;

            case 4:
                SorteandoQuaisBuffs(botoesCritico);
                break;

            case 5:
                SorteandoQuaisBuffs(botoesMovimentacao);
                break;
        }
    }

    private void SorteandoQuaisBuffs(GameObject[] buffs)
    {
        int aux1 = Random.Range(0, buffs.Length);
        int aux2 = Random.Range(0, buffs.Length);
        int aux3 = Random.Range(0, buffs.Length);

        buffs[aux1].SetActive(true);
        buffs[aux2].SetActive(true);
        buffs[aux3].SetActive(true);
    }

    public void DesativandoTodosOsBotoes()
    {
        for(int i = 0; i < botoesBurst.Length; i++)
        {
            botoesBurst[i].SetActive(false);
        }

        for (int i = 0; i < botoesDefesa.Length; i++)
        {
            botoesDefesa[i].SetActive(false);
        }

        for (int i = 0; i < botoesCritico.Length; i++)
        {
            botoesCritico[i].SetActive(false);
        }

        for (int i = 0; i < botoesMovimentacao.Length; i++)
        {
            botoesMovimentacao[i].SetActive(false);
        }

        for (int i = 0; i < botoesCorrosao.Length; i++)
        {
            botoesCorrosao[i].SetActive(false);
        }
    }
}
