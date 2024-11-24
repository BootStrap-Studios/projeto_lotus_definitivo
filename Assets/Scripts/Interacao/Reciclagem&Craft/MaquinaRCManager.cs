using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaquinaRCManager : MonoBehaviour
{
    [Header("Config Maquina RC Manager")]
    public GameObject maquinaRC_UI;
    [SerializeField] private GameObject itemInventario;
    [SerializeField] private GameObject posItens;
    [SerializeField] private GameObject posItens2;
    [SerializeField] private Collider maquinaRCTrigger;
    public enum StateRC
    {
        inicio,
        reciclar,
        fabricar,
        itemSelecionado,
        fechado
    };
    public StateRC stateRC;
    private StateRC stateRCAnterior;
    private bool qualAcao;

    [Header("UI")]
    [SerializeField] private GameObject inicioUI;
    [SerializeField] private GameObject reciclagemUI;
    [SerializeField] private GameObject fabricacaoUI;
    [SerializeField] private MaquinaRCItem[] itensReciclaveis;
    [SerializeField] private MaquinaRCItem[] itensFabricaveis;
    
    //Inventário
    private InventarioSystem inventarioSystem;
    private GameObject[] itensInventario;
    private bool mudaSpawn;

    private void Awake()
    {
        inventarioSystem = FindObjectOfType<InventarioSystem>();
    }

    private void Update()
    {
        /*if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (stateRC == StateRC.inicio)
            {
                FecharUI();
            }
            else if(stateRC == StateRC.reciclar)
            {
                VoltarInicio();
            }
            else if(stateRC == StateRC.fabricar)
            {
                VoltarInicio();
            }
            else if(stateRC == StateRC.itemSelecionado)
            {
                DeselecionaItem();          
            }
        }     */ 
    }

    public void DesabilitaOutrosItens(int idItem)
    {
        if (qualAcao)
        {
            for (int i = 0; i < itensReciclaveis.Length; i++)
            {
                if (itensReciclaveis[i].idItem != idItem)
                {
                    itensReciclaveis[i].itemDesabilitadoIMG.enabled = true;
                    itensReciclaveis[i].botaoItem.interactable = false;
                }
            }
        }
        else
        {
            for (int i = 0; i < itensFabricaveis.Length; i++)
            {
                if (itensFabricaveis[i].idItem != idItem)
                {
                    itensFabricaveis[i].itemDesabilitadoIMG.enabled = true;
                    itensFabricaveis[i].botaoItem.interactable = false;
                }
            }
        }
    }

    #region Funções Botões

    public void FecharUI()
    {
        maquinaRC_UI.SetActive(true);
        maquinaRCTrigger.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        stateRC = StateRC.fechado;
    }

    public void VoltarInicio()
    {
        inicioUI.SetActive(true);
        reciclagemUI.SetActive(false);
        fabricacaoUI.SetActive(false);

        stateRC = StateRC.inicio;
    }

    public void Reciclar()
    {
        inicioUI.SetActive(false);
        reciclagemUI.SetActive(true);
        fabricacaoUI.SetActive(false);

        qualAcao = true;

        for(int i = 0; i < itensReciclaveis.Length; i++)
        {
            itensReciclaveis[i].itemDesabilitadoIMG.enabled = false;
            itensReciclaveis[i].botaoItem.interactable = true;
        }

        stateRC = StateRC.reciclar;
        stateRCAnterior = stateRC;
    }

    public void Fabricar()
    {
        inicioUI.SetActive(false);
        reciclagemUI.SetActive(false);
        fabricacaoUI.SetActive(true);

        qualAcao = false;

        for (int i = 0; i < itensFabricaveis.Length; i++)
        {
            itensFabricaveis[i].itemDesabilitadoIMG.enabled = false;
            itensFabricaveis[i].botaoItem.interactable = true;
        }

        stateRC = StateRC.fabricar;
        stateRCAnterior = stateRC;
    }

    public void DeselecionaItem()
    {
        if (stateRCAnterior == StateRC.reciclar)
        {
            Reciclar();
            for (int i = 0; i < itensReciclaveis.Length; i++)
            {
                itensReciclaveis[i].DeselecionaItem();
            }
        }
        else if (stateRCAnterior == StateRC.fabricar)
        {
            Fabricar();
            for (int i = 0; i < itensFabricaveis.Length; i++)
            {
                itensReciclaveis[i].DeselecionaItem();
            }
        }
        else
        {
            Debug.LogError("Não deveria estar passando aqui!");
        }
    }

    #endregion

    #region Itens do Inventário

    public void CarregaInventario()
    {
        itensInventario = new GameObject[inventarioSystem.listaItens.Length];

        for (int i = 0; i < itensInventario.Length; i++)
        {
            if (!mudaSpawn)
            {
                itensInventario[i] = Instantiate(itemInventario, posItens.transform);
                mudaSpawn = true;
            }
            else
            {
                itensInventario[i] = Instantiate(itemInventario, posItens2.transform);
                mudaSpawn = false;
            }

            itensInventario[i].GetComponent<ItemInventario>().imagem.sprite = inventarioSystem.spritesItens[i].sprite;
        }
    }

    public void AtualizaInventario()
    {
        for (int i = 0; i < itensInventario.Length; i++)
        {
            itensInventario[i].GetComponent<ItemInventario>().AtualizaTexto(inventarioSystem.listaItens[i].nomeItem, inventarioSystem.quantidadeTotalItem[i]);
        }
    }

    #endregion
}
