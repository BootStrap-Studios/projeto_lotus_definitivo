using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LinhaBuffPermanente : MonoBehaviour, IUpdateSelectedHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Config Linha de Buffs")]
    [SerializeField] private Image buffDesbloqueado;
    [SerializeField] private ItemDropado[] nomeItensGastos;
    [SerializeField] private int[] qntdItensGastos;
    [SerializeField] private LevelBuff[] leveisBuff;
    public int idBuff;
    public int levelAtualBuff;
    public bool desbloqueado;
    private bool pressionado;
    private float tempoPressionando;   

    [Header("UI")]
    public string titulo;
    public string descricao;
    public string[] descricaoBuff;

    [Header("Outros Scripts")]
    [SerializeField] private InventarioSystem inventarioSystem;
    [SerializeField] private BuffsPermanenteManager buffsPermanenteManager;

    [SerializeField] MargelaH_CAM cam;

    public void OnUpdateSelected(BaseEventData baseEvent)
    {
        if (pressionado)
        {
            if (inventarioSystem.ConfereRecursos(nomeItensGastos, qntdItensGastos, false))
            {
                tempoPressionando = tempoPressionando + Time.unscaledDeltaTime;
                buffDesbloqueado.fillAmount -= Time.unscaledDeltaTime / 2;

                if (tempoPressionando >= 2)
                {  
                    inventarioSystem.ConfereRecursos(nomeItensGastos, qntdItensGastos, true);
                    buffsPermanenteManager.AtualizaInventario();
                    desbloqueado = true;
                    LiberaBuffs();
                }
            }
            else
            {
                gameObject.GetComponent<Button>().interactable = false;
                buffsPermanenteManager.MsgErro("Você não tem todos os recursos necessários para esta ação", gameObject.transform.position);
            }
        }
        else
        {
            buffDesbloqueado.fillAmount = 1;
            tempoPressionando = 0;
        }
    }

    public void OnPointerDown(PointerEventData pointerEvent)
    {
        pressionado = true;
    }

    public void OnPointerUp(PointerEventData pointerEvent)
    {
        pressionado = false;

        buffDesbloqueado.fillAmount = 1;
        tempoPressionando = 0;
    }

    public void OnPointerEnter(PointerEventData pointerEvent)
    {
        buffsPermanenteManager.MostraDebito(qntdItensGastos, nomeItensGastos, true);

        buffsPermanenteManager.AtualizaUI(titulo, descricao, "Desbloqueie para Melhorar");
    }

    public void OnPointerExit(PointerEventData pointerEvent)
    {
        buffsPermanenteManager.MostraDebito(qntdItensGastos, nomeItensGastos, false);
    }

    public bool ConfereLevel(int levelBuff)
    {
        //Debug.Log(levelAtualBuff + " <-> " + levelBuff);
        if(levelAtualBuff == levelBuff)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void LiberaBuffs()
    {
        buffDesbloqueado.gameObject.SetActive(false);
        gameObject.GetComponent<Button>().interactable = false;

        //Debug.Log("Desbloquiei arvore de buffs");
    }

    public void AtulizaBuffsDesbloqueados(int levelDesbloqueado)
    {
        for(int i = 0; i < levelDesbloqueado; i++)
        {
            leveisBuff[i].LiberaBuff();
        }

        LiberaBuffs();
        levelAtualBuff++;
    }
}
