using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropado : MonoBehaviour
{
    [SerializeField] private string nomeItem;
    [SerializeField] private float quantidadeItem;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EventBus.Instance.ColetaItem(nomeItem, quantidadeItem);
            Destroy(gameObject);
        }
    }
}
