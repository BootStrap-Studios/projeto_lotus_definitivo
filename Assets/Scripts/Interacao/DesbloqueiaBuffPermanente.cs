using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DesbloqueiaBuffPermanente : MonoBehaviour, IUpdateSelectedHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Image buffDesbloqueado;
    [SerializeField] private string[] nomeItensGastos;
    [SerializeField] private int[] qntdItensGastos;
    private InventarioSystem inventarioSystem;
    private float tempoPressionando;
    private bool pressionado;
    private bool tenhoRecursos;

    private void Awake()
    {
        inventarioSystem = FindObjectOfType<InventarioSystem>();
    }

    public void OnUpdateSelected(BaseEventData baseEvent)
    {
        if (pressionado && tenhoRecursos)
        {
            tempoPressionando = tempoPressionando + Time.unscaledDeltaTime;
            buffDesbloqueado.fillAmount -= Time.unscaledDeltaTime / 2;

            if (tempoPressionando >= 2)
            {
                buffDesbloqueado.gameObject.SetActive(false);
                gameObject.GetComponent<Button>().interactable = false;
                inventarioSystem.ConfereRecursos(nomeItensGastos, qntdItensGastos, true);
            }
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

    public void VerificaRecursos()
    {
        tenhoRecursos = inventarioSystem.ConfereRecursos(nomeItensGastos, qntdItensGastos, false);
    }

}
