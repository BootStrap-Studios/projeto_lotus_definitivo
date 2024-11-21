using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LinhaBuffPermanente : MonoBehaviour, IUpdateSelectedHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Config Linha de Buffs")]
    [SerializeField] private Image buffDesbloqueado;
    [SerializeField] private ItemDropado[] nomeItensGastos;
    [SerializeField] private int[] qntdItensGastos;
    [SerializeField] private LevelBuff[] leveisBuff;
    public int idBuff;
    public int levelAtualBuff;
    private float tempoPressionando;
    private bool pressionado;
    private bool tenhoRecursos;

    [Header("UI")]
    public string titulo;
    public string descricao;

    [Header("Outros Scripts")]
    [SerializeField] private InventarioSystem inventarioSystem;
    [SerializeField] private BuffsPermanenteManager buffsPermanenteManager;
    

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
                buffsPermanenteManager.AtualizaInventario();
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

    public bool ConfereLevel(int levelBuff)
    {
        if(levelAtualBuff == levelBuff)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
