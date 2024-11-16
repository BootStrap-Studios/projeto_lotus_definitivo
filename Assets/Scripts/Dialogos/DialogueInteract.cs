using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private TextAsset inkJson;
    public void Interagir()
    {
        if(!DialogueManager.instance.dialogueIsPlaying)
        {
            DialogueManager.instance.EnterDialogueMode(inkJson);
        }
        
    }
}
