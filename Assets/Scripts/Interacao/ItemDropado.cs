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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            int quantidadeItem = Random.Range(1, quantidadeItemMax + 1);

            EventBus.Instance.ColetaItem(nomeItem, quantidadeItem, true);
            Destroy(gameObject);
        }
    }
}
