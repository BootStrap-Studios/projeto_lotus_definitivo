using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class TerminalBuff : MonoBehaviour, IInteractable
{
    public enum PackRecompensas
    {
        SO_BUFF,
        BUFF_RECURSO,
        BUFF_DADO_CURA,
        SO_ULT,
        BUFF_RECURSO_DADO,
        BUFF_CURA,
        BUFF_RECURSO_CURA
    }

    private BuffManager buffManager;

    [SerializeField] private GameObject elevador;

    [SerializeField] private PackRecompensas pack;

    private VidaPlayer vidaPlayer;

    private InventarioSystem inventarioSystem;

    void Start()
    {
        buffManager = FindObjectOfType<BuffManager>();

        vidaPlayer = FindObjectOfType<VidaPlayer>();

        inventarioSystem = FindObjectOfType<InventarioSystem>();
    }

    public void Interagir()
    {
        SorteioRecompensa();


        gameObject.GetComponent<BoxCollider>().enabled = false;
        elevador.SetActive(true);
    }

    private void SorteioRecompensa()
    {
        switch (pack)
        {
            case PackRecompensas.SO_BUFF:
                Buff();

                break;

            case PackRecompensas.BUFF_RECURSO:

                int aux = Random.Range(1, 3);

                if(aux == 1)
                {
                    Buff();
                }
                if(aux == 2)
                {
                    Recurso();
                }

                break;

            case PackRecompensas.BUFF_DADO_CURA:

                int aux1 = Random.Range(1, 4);

                if (aux1 == 1)
                {
                    Buff();
                }
                if (aux1 == 2)
                {
                    Cura();
                }
                if(aux1 == 3)
                {
                    Dados();
                }
                break;

            case PackRecompensas.SO_ULT:

                Ultimate();
                break;

            case PackRecompensas.BUFF_RECURSO_DADO:

                int aux2 = Random.Range(1, 4);

                if (aux2 == 1)
                {
                    Buff();
                }
                if (aux2 == 2)
                {
                    Recurso();
                }
                if (aux2 == 3)
                {
                    Dados();
                }
                break;

            case PackRecompensas.BUFF_CURA:

                int aux3 = Random.Range(1, 3);

                if (aux3 == 1)
                {
                    Buff();
                }
                if (aux3 == 2)
                {
                    Cura();
                }
                break;

            case PackRecompensas.BUFF_RECURSO_CURA:

                int aux4 = Random.Range(1, 4);

                if (aux4 == 1)
                {
                    Buff();
                }
                if (aux4 == 2)
                {
                    Recurso();
                }
                if (aux4 == 3)
                {
                    Cura();
                }

                break;
        }

    }


    private void Buff()
    {
        Debug.Log("Sorteou buff hein");
        EventBus.Instance.PodePausar(false);
        EventBus.Instance.PauseGame();

        buffManager.SorteandoQualArvore();
        AudioManager.instance.PlayOneShot(FMODEvents.instance.abrirTerminalBuffs, transform.position);

        Time.timeScale = 0;
    }

    private void Recurso()
    {
        //Contabilizando quantos itens prticiparam do sorteio
        int qualItem = 0;

        for (int i = 0; i < inventarioSystem.listaItens.Length; i++)
        {
            if (inventarioSystem.listaItens[i].recompensaSala)
            {
                qualItem++;
            }
        }
        Debug.Log(qualItem + " serão adicionados ao sorteio!");

        //Adicionando os itens para o sorteio
        ItemDropado[] item = new ItemDropado[qualItem];
        qualItem = 0;

        for (int i = 0; i < inventarioSystem.listaItens.Length; i++)
        {
            if (inventarioSystem.listaItens[i].recompensaSala)
            {
                item[qualItem] = inventarioSystem.listaItens[i];
                Debug.Log(item[qualItem].nomeItem + " foi adicionado ao sorteio!");
                qualItem++;
            }
        }

        //Sorteando recurso
        int numSorteado = Random.Range(0, qualItem);
        Debug.Log(item[numSorteado].nomeItem + " foi o recurso sorteado!");

        //Adicionando ao inventário o item novo
        EventBus.Instance.ColetaItem(item[numSorteado].nomeItem, 15, true);

    }

    private void Dados()
    {
        Debug.Log("Sorteou dados hein");

        EventBus.Instance.ColetaItem("Dados de Projeto", 1, true);
    }

    private void Cura()
    {
        Debug.Log("Sorteou cura hein");
        vidaPlayer.CurarVida(5f);
    }

    private void Ultimate()
    {
        Debug.Log("Sorteou ultimate hein");
        //AINDA NAO TERMINEI AS ULTS :)))))))))))))))
    }
}
