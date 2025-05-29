using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDropado : MonoBehaviour
{ 
    [SerializeField] private int quantidadeItemMax;
    public string nomeItem;
    public Image spriteItem;
    public bool recompensaSala;
    [SerializeField] Vector3 cenario;
    float muiplicadorPos = 0;

    private void OnEnable()
    {
        EventBus.Instance.onGameOver += DeletarItem;
    }

    private void OnDisable()
    {
        EventBus.Instance.onGameOver -= DeletarItem;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            int quantidadeItem = Random.Range(1, quantidadeItemMax + 1);

            EventBus.Instance.ColetaItem(nomeItem, quantidadeItem, true);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Inimigo"))
        {
            return;
        }
        else
        {
            Debug.Log("Item bugado, movendo item...");
            cenario = other.transform.position;
            MoverItem();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            return;
        }
        else if (other.CompareTag("Inimigo"))
        {
            return;
        }
        else
        {
            Debug.Log("Item bugado, movendo item...");
            cenario = other.transform.position;
            MoverItem();
        }
    }

    [ContextMenu("Mover Item")]
    private void MoverItem()
    {
        Vector3 novoPos = cenario - gameObject.transform.position;
        novoPos = new Vector3(novoPos.x + muiplicadorPos, novoPos.y, novoPos.z + muiplicadorPos);
        gameObject.transform.position = new Vector3(novoPos.x + muiplicadorPos, gameObject.transform.position.y, novoPos.z + muiplicadorPos);
        muiplicadorPos += 0.2f;
        Debug.Log(muiplicadorPos);
    }

    private void DeletarItem()
    {
        Destroy(gameObject);
    }
}
