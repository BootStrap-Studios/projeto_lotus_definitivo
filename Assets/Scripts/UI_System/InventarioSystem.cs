using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class InventarioSystem : MonoBehaviour, ISave
{
    [Header("Listas dos Itens")]
    public ItemDropado[] listaItens;
    private Image[] spritesItens;
    private TextMeshProUGUI[] quantidadeTotalItemUI;    
    public float[] quantidadeTotalItem;
    

    [Header("Item Atual")]
    [SerializeField] private GameObject posItemAtual;
    [SerializeField] private ItemColetado itemAtual;

    [Header("Outros")]
    [SerializeField] private GameObject objSpriteList;
    [SerializeField] private GameObject objStringList;    
    [SerializeField] private TextMeshProUGUI textoBase;

    private void OnEnable()
    {
        EventBus.Instance.onColetaItem += VerificaListaItens;
    }

    private void OnDisable()
    {
        EventBus.Instance.onColetaItem -= VerificaListaItens;
    }

    private void Start()
    {
        spritesItens = new Image[listaItens.Length];
        quantidadeTotalItemUI = new TextMeshProUGUI[listaItens.Length];
        quantidadeTotalItem = new float[listaItens.Length];

        for (int i = 0; i < listaItens.Length; i++)
        {
            spritesItens[i] = Instantiate(listaItens[i].spriteItem, objSpriteList.transform);
            spritesItens[i].gameObject.SetActive(false);

            quantidadeTotalItemUI[i] = Instantiate(textoBase, objStringList.transform);
            quantidadeTotalItemUI[i].gameObject.SetActive(false);     
        }

        SaveSystemManager.instance.CarregarJogo();
    }

    private void VerificaListaItens(string nomeItem, float qntdItem, bool itemColetado)
    {        
        for (int i = 0; i < listaItens.Length; i++)
        {
            if (listaItens[i].nomeItem == nomeItem)
            {
                if (itemColetado)
                {                    
                    itemAtual.ConfigurandoItem(spritesItens[i].color, nomeItem + ": " + qntdItem.ToString());
                    Instantiate(itemAtual.gameObject, posItemAtual.transform);
                }

                quantidadeTotalItem[i] += qntdItem;
                quantidadeTotalItemUI[i].text = nomeItem + ": " + quantidadeTotalItem[i].ToString();

                spritesItens[i].gameObject.SetActive(true);
                quantidadeTotalItemUI[i].gameObject.SetActive(true);

                break;
            }
        }           
    }

    public void CarregarSave(InfosSave save)
    {
        for (int i = 0; i < quantidadeTotalItem.Length; i++)
        {
            quantidadeTotalItem[i] = save.quantidadeItem[i];
            quantidadeTotalItemUI[i].text = listaItens[i].nomeItem + ": " + quantidadeTotalItem[i].ToString();
            //Debug.Log(listaItens[i].nomeItem + ": " + quantidadeTotalItem[i]);

            if (quantidadeTotalItem[i] > 0)
            {
                spritesItens[i].gameObject.SetActive(true);
                quantidadeTotalItemUI[i].gameObject.SetActive(true);
            }
        }
    }

    public void SalvarSave(ref InfosSave save)
    {
        for(int i = 0; i < quantidadeTotalItem.Length; i++)
        {
            save.quantidadeItem[i] = quantidadeTotalItem[i];
        }
    }
}

