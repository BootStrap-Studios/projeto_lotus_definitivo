using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;

public class LevelBuff : MonoBehaviour, IUpdateSelectedHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Config Buff")]
    [SerializeField] private Image buffDesbloqueado;
    [SerializeField] private int levelBuff;
    [SerializeField] private ItemDropado[] itensGastos;
    [SerializeField] private int[] qntdItensGastos;
    public bool levelComprado;
    private bool pressionado;
    private float tempoPressionando;

    [Header("Outros Scripts")]
    [SerializeField] private BuffsPermanenteManager buffsPermanenteManager;
    [SerializeField] private LinhaBuffPermanente linhaBuffPermanente;
    [SerializeField] private InventarioSystem inventarioSystem;

    public void OnUpdateSelected(BaseEventData baseEvent)
    {
        if (pressionado)
        {
            if (linhaBuffPermanente.ConfereLevel(levelBuff))
            {
                if (inventarioSystem.ConfereRecursos(itensGastos, qntdItensGastos, false))
                {
                    tempoPressionando = tempoPressionando + Time.unscaledDeltaTime;
                    buffDesbloqueado.fillAmount += Time.unscaledDeltaTime / 2;

                    if (tempoPressionando >= 2)
                    {
                        buffDesbloqueado.fillAmount = 1;
                        buffDesbloqueado.raycastTarget = true;
                        buffsPermanenteManager.QualBuff(linhaBuffPermanente.idBuff, levelBuff);
                        inventarioSystem.ConfereRecursos(itensGastos, qntdItensGastos, true);
                        buffsPermanenteManager.AtualizaInventario();
                        gameObject.GetComponent<Button>().interactable = false;
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
                gameObject.GetComponent<Button>().interactable = false;
                buffsPermanenteManager.MsgErro("Desbloqueie outras habilidades antes desta", gameObject.transform.position);
            }
        }
        else
        {
            buffDesbloqueado.fillAmount = 0;
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
        gameObject.GetComponent<Button>().interactable = true;
    }

    public void OnPointerEnter(PointerEventData pointerEvent)
    {
        buffsPermanenteManager.MostraDebito(qntdItensGastos, itensGastos, true);

        buffsPermanenteManager.AtualizaUI(linhaBuffPermanente.titulo, linhaBuffPermanente.descricao, linhaBuffPermanente.descricaoBuff[levelBuff - 1]);
    }

    public void OnPointerExit(PointerEventData pointerEvent)
    {
        buffsPermanenteManager.MostraDebito(qntdItensGastos, itensGastos, false);
    }

    public void ConfereBuff()
    {

    }
}
