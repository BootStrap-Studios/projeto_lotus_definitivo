using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MaquinaRCItem : MonoBehaviour
{
    [Header("Config Item")]
    public int idItem;
    public Image itemDesabilitadoIMG;
    public Button botaoItem;
    [SerializeField] private Button botaoConfirmar;
    [SerializeField] private ItemDropado[] itensGastos;
    [SerializeField] private ItemDropado itemGanho;
    [SerializeField] private int[] quantidadeItemGastos;
    private int[] quantidadeMultiplicada;
    private int quantidade;
    public bool pressionado;
    private float tempoPressionado;

    [Header("UI")]
    [SerializeField] private GameObject[] botoesAux;
    [SerializeField] private TextMeshProUGUI quantidadeTXT;

    [Header("Outros Scripts")]
    [SerializeField] private MaquinaRCManager maquinaRCManager;
    [SerializeField] private InventarioSystem inventarioSystem;

    private void Awake()
    {
        quantidadeMultiplicada = new int[quantidadeItemGastos.Length];
    }

    public void Update()
    {
        if (pressionado)
        {
            for (int i = 0; i < quantidadeItemGastos.Length; i++)
            {
                quantidadeMultiplicada[i] = quantidadeItemGastos[i] * quantidade;
            }

            if (inventarioSystem.ConfereRecursos(itensGastos, quantidadeMultiplicada, false))
            {
                tempoPressionado = tempoPressionado + Time.unscaledDeltaTime;
                itemDesabilitadoIMG.fillAmount -= Time.unscaledDeltaTime / 2;

                if(tempoPressionado > 2)
                {
                    inventarioSystem.ConfereRecursos(itensGastos, quantidadeMultiplicada, true);
                    inventarioSystem.ReciclagemECraft(itemGanho, quantidade);
                    maquinaRCManager.AtualizaInventario();
                    quantidade = 0;
                    quantidadeTXT.text = "0x";
                }
            }
            else
            {
                botaoConfirmar.interactable = false;
                maquinaRCManager.MsgErro("Você não tem todos os recursos necessários para esta ação", gameObject.transform.position);
            }
        }
        else
        {
            tempoPressionado = 0;
            itemDesabilitadoIMG.fillAmount = 0;
        }
    }

    public void SelecionaItem()
    {
        maquinaRCManager.DesabilitaOutrosItens(idItem);
        maquinaRCManager.stateRC = MaquinaRCManager.StateRC.itemSelecionado;

        quantidadeTXT.text = "0x";
        quantidade = 0;

        for (int i = 0; i < botoesAux.Length; i++)
        {
            botoesAux[i].SetActive(true);
        }
    }

    public void DeselecionaItem()
    {
        for (int i = 0; i < botoesAux.Length; i++)
        {
            botoesAux[i].SetActive(false);
        }

        maquinaRCManager.MostraDebito(quantidadeItemGastos, itensGastos, false);
    }

    public void AumentaQuantidade()
    {
        quantidade++;
        quantidadeTXT.text = quantidade.ToString() + "x";

        for (int i = 0; i < quantidadeItemGastos.Length; i++)
        {
            quantidadeMultiplicada[i] = quantidadeItemGastos[i] * quantidade;
        }

        maquinaRCManager.MostraDebito(quantidadeMultiplicada, itensGastos, true);
    }

    public void DiminuiQuantidade()
    {
        if (quantidade - 1 > 0)
        {
            quantidade--;
            quantidadeTXT.text = quantidade.ToString() + "x";


            for (int i = 0; i < quantidadeItemGastos.Length; i++)
            {
                quantidadeMultiplicada[i] = quantidadeItemGastos[i] * quantidade;
            }

            maquinaRCManager.MostraDebito(quantidadeMultiplicada, itensGastos, true);  
        }
        else
        {
            quantidade = 0;
            quantidadeTXT.text = quantidade.ToString() + "x";
            maquinaRCManager.MostraDebito(quantidadeItemGastos, itensGastos, false);
        }
    }
}
