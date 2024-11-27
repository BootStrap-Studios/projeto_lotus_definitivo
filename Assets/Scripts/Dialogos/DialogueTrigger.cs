using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private TextAsset inkJSON;
    [SerializeField] private GameObject primeiraViagem;
    [SerializeField] private GameObject toqueDeRecolher;
    [SerializeField] private GameObject disposicaoTotal;

    public bool ativarPrimeiraViagem;
    public bool tirarToqueDeRecolher;
    public bool puxarDisposicaoTotal;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(ativarPrimeiraViagem)
            {
                primeiraViagem.SetActive(true);
            }
            if(tirarToqueDeRecolher)
            {
                if(toqueDeRecolher != null)
                {
                    Destroy(toqueDeRecolher);
                }
            }
            DialogueManager.instance.EnterDialogueMode(inkJSON);

            Destroy(gameObject);
        }
    }
}
