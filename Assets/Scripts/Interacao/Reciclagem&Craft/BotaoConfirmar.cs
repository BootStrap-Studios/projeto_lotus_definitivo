using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BotaoConfirmar : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private MaquinaRCItem maquinaRCItem;

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        maquinaRCItem.pressionado = true;
        maquinaRCItem.itemDesabilitadoIMG.fillAmount = 1;
    }

    public void OnPointerUp(PointerEventData pointerEventData)
    {
        maquinaRCItem.pressionado = false;
        gameObject.GetComponent<Button>().interactable = true;
    }
}
