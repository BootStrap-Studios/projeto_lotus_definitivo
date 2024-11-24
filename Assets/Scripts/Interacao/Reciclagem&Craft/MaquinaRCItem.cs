using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MaquinaRCItem : MonoBehaviour
{
    [Header("Config Item")]
    public int idItem;
    public Image itemDesabilitadoIMG;
    public Button botaoItem;
    private int quantidade;

    [Header("UI")]
    [SerializeField] private GameObject[] botoesAux;
    [SerializeField] private TextMeshProUGUI quantidadeTXT;

    [Header("Outros Scripts")]
    [SerializeField] private MaquinaRCManager maquinaRCManager;
    [SerializeField] private InventarioSystem inventarioSystem;

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
    }

    public void AumentaQuantidade()
    {
        quantidade++;
        quantidadeTXT.text = quantidade.ToString() + "x";
    }

    public void DiminuiQuantidade()
    {
        if(quantidade > 0)
        {
            quantidade--;
            quantidadeTXT.text = quantidade.ToString() + "x";
        }
    }
}
