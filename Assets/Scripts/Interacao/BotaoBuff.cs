using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BotaoBuff : MonoBehaviour
{
    [SerializeField] private int qualArvore;
    

    private Button button;
    private BuffManager buffManager;

    private void RetirarBuff()
    {
        Debug.Log("Retirando buff");
        buffManager.RetirandoBuffDaLista(qualArvore, this.gameObject);
    }

    private void OnEnable()
    {
        button = GetComponent<Button>();

        buffManager = FindObjectOfType<BuffManager>();

        button.onClick.AddListener(() => RetirarBuff());
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(() => RetirarBuff());
    }

}
