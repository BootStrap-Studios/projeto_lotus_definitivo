using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MaquinaRCManager : MonoBehaviour
{
    [Header("Config Maquina RC Manager")]
    public GameObject maquinaRC_UI;
    [SerializeField] private GameObject itemInventario;
    [SerializeField] private GameObject posItens;
    [SerializeField] private GameObject posItens2;
    [SerializeField] private Collider maquinaRCTrigger;
    [SerializeField] private GameObject itensInventario;
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
    [SerializeField] private TextMeshProUGUI msgErroTXT;
    private float tempoMsgAtiva;
    private bool msgErroAtiva;
    
    //Inventário
    private InventarioSystem inventarioSystem;   
    private GameObject[] itens;
    private bool mudaSpawn;

    private void Awake()
    {
        inventarioSystem = FindObjectOfType<InventarioSystem>();
    }

    private void Update()
    {
        if (msgErroAtiva)
        {
            msgErroTXT.gameObject.transform.position = new Vector3(msgErroTXT.gameObject.transform.position.x, msgErroTXT.gameObject.transform.position.y + (Time.unscaledDeltaTime / 0.03f), msgErroTXT.gameObject.transform.position.z);

            tempoMsgAtiva -= Time.unscaledDeltaTime;

            if (tempoMsgAtiva <= 0)
            {
                msgErroTXT.color = new Color(msgErroTXT.color.r, msgErroTXT.color.g, msgErroTXT.color.b, msgErroTXT.color.a - (Time.unscaledDeltaTime / 0.7f));

                if (msgErroTXT.color.a <= 0)
                {
                    msgErroAtiva = false;
                    msgErroTXT.enabled = false;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
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
        }
    }

    public void DesabilitaOutrosItens(int idItem)
    {
        if (qualAcao)
        {
            for (int i = 0; i < itensReciclaveis.Length; i++)
            {
                if (itensReciclaveis[i].idItem != idItem)
                {
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
                    itensFabricaveis[i].botaoItem.interactable = false;
                }
            }
        }
    }

    public void MsgErro(string msgErro, Vector3 posMsg)
    {
        msgErroTXT.text = msgErro;
        msgErroTXT.transform.position = posMsg;

        tempoMsgAtiva = 1.5f;
        msgErroTXT.color = new Color(msgErroTXT.color.r, msgErroTXT.color.g, msgErroTXT.color.b, 1);
        msgErroTXT.enabled = true;

        msgErroAtiva = true;
    }

    public void MostraDebito(int[] debitos, ItemDropado[] nomesItens, bool ativaDebito)
    {
        for (int j = 0; j < nomesItens.Length; j++)
        {
            for (int i = 0; i < itens.Length; i++)
            {
                if (nomesItens[j].nomeItem == inventarioSystem.listaItens[i].nomeItem)
                {
                    itens[i].GetComponent<ItemInventario>().MostraDebito(debitos[j], ativaDebito);
                }
            }
        }
    }

    #region Funções Botões

    public void FecharUI()
    {
        maquinaRC_UI.SetActive(false);
        maquinaRCTrigger.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        stateRC = StateRC.fechado;
        EventBus.Instance.PauseGame();
        EventBus.Instance.PodePausar(true);
        Time.timeScale = 1;
    }

    public void VoltarInicio()
    {
        DeselecionaItem();

        inicioUI.SetActive(true);
        reciclagemUI.SetActive(false);
        fabricacaoUI.SetActive(false);
        itemInventario.SetActive(false);

        stateRC = StateRC.inicio;       
    }

    public void Reciclar()
    {
        inicioUI.SetActive(false);
        reciclagemUI.SetActive(true);
        fabricacaoUI.SetActive(false);
        itemInventario.SetActive(true);

        qualAcao = true;

        for(int i = 0; i < itensReciclaveis.Length; i++)
        {
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
        itemInventario.SetActive(true);

        qualAcao = false;

        for (int i = 0; i < itensFabricaveis.Length; i++)
        {
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
                itensFabricaveis[i].DeselecionaItem();
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
        itens = new GameObject[inventarioSystem.listaItens.Length];

        for (int i = 0; i < itens.Length; i++)
        {
            if (!mudaSpawn)
            {
                itens[i] = Instantiate(itensInventario, posItens.transform);
                mudaSpawn = true;
            }
            else
            {
                itens[i] = Instantiate(itensInventario, posItens2.transform);
                mudaSpawn = false;
            }

            itens[i].GetComponent<ItemInventario>().imagem.sprite = inventarioSystem.spritesItens[i].sprite;
        }
    }

    public void AtualizaInventario()
    {
        for (int i = 0; i < itens.Length; i++)
        {
            itens[i].GetComponent<ItemInventario>().AtualizaTexto(inventarioSystem.listaItens[i].nomeItem, inventarioSystem.quantidadeTotalItem[i]);
        }
    }

    #endregion
}
