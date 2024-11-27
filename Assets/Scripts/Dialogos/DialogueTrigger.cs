using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private TextAsset inkJSON;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            DialogueManager.instance.EnterDialogueMode(inkJSON);

            Destroy(gameObject);
        }
    }
}
