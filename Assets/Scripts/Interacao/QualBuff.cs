using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;

public class QualBuff : MonoBehaviour, IUpdateSelectedHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Config Buff")]
    [SerializeField] private Image buffDesbloqueado;
    [SerializeField] private int idBuff;
    [SerializeField] private string nomeBuff;
    [SerializeField] private int levelBuff;
    [SerializeField] private string[] nomeItensGastos;
    [SerializeField] private int[] qntdItensGastos;
    public bool levelComprado;
    private bool levelDesbloqueado;
    private bool pressionado;
    private float tempoPressionando;
    private bool tenhoRecursos;

    [Header("Outros Scripts")]
    [SerializeField] private BuffsPermanenteManager buffsPermanenteManager;
    [SerializeField] private InventarioSystem inventarioSystem;



    public void OnUpdateSelected(BaseEventData baseEvent)
    {
        if (pressionado)
        {
            tenhoRecursos = inventarioSystem.ConfereRecursos(nomeItensGastos, qntdItensGastos, false);
            levelDesbloqueado = buffsPermanenteManager.LevelDesbloqueado(idBuff, levelBuff);

            if (tenhoRecursos && levelDesbloqueado)
            {
                tempoPressionando = tempoPressionando + Time.unscaledDeltaTime;
                buffDesbloqueado.fillAmount += Time.unscaledDeltaTime / 2;

                if (tempoPressionando >= 2)
                {
                    buffDesbloqueado.fillAmount = 1;
                    buffDesbloqueado.raycastTarget = true;
                    levelComprado = true;
                    buffsPermanenteManager.QualBuff(idBuff, levelBuff);
                    inventarioSystem.ConfereRecursos(nomeItensGastos, qntdItensGastos, true);
                }
            }
            else
            {
                gameObject.GetComponent<Button>().interactable = false;
            }
        }
        else
        {
            if (!levelComprado) buffDesbloqueado.fillAmount = 0;
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

    public void ConfereBuff()
    {

    }
}
