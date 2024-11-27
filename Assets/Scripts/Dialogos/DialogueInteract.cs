using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private TextAsset inkJson;

    [SerializeField] private GameObject terminalBuff;
    [SerializeField] private GameObject finalizarOGame;

    public bool dialogoSala0 = false;
    public bool ultimoDialogo;
    public void Interagir()
    {
        if(!DialogueManager.instance.dialogueIsPlaying)
        {
            DialogueManager.instance.EnterDialogueMode(inkJson);
        }

        if(dialogoSala0)
        {
            terminalBuff.SetActive(true);
        }

        if(ultimoDialogo)
        {
            finalizarOGame.SetActive(true);
        }

        Destroy(gameObject);
    }
}
