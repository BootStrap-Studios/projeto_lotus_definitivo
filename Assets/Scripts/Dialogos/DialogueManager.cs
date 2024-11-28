using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance { get; private set; }


    [Header("Dialogue UI")]

    [SerializeField] private GameObject dialogueUI;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI displayNameText;
    [SerializeField] private Animator portraitAnimator;

    [SerializeField] private GameObject playerUI;

    [Header("PrimeiroDialogo")]
    [SerializeField] private TextAsset primeiroDialogo;

    private StatusJogador statusJogador;

    private Story currentStory;

    public bool dialogueIsPlaying { get; private set; }

    private const string SPEAKER_TAG = "speaker";

    private const string ANIM_TAG = "anim";

    private const string TP_TAG = "tp";



    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
        }

        instance = this;
    }

    private void Start()
    {
        statusJogador = FindObjectOfType<StatusJogador>();

        dialogueIsPlaying = false;

        dialogueUI.SetActive(false);

        EnterDialogueMode(primeiroDialogo);


    }

    private void Update()
    {
        if(!dialogueIsPlaying)
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Space))
        {
            ContinueStory();
        }
    }

    public void EnterDialogueMode(TextAsset inkJSON)
    {
        EventBus.Instance.PodePausar(false);
        EventBus.Instance.PauseGame();
        Time.timeScale = 0;

        currentStory = new Story(inkJSON.text);

        dialogueIsPlaying = true;
        dialogueUI.SetActive(true);
        playerUI.SetActive(false);

        ContinueStory();
    }

    private void ExitDialogueMode()
    {
        Time.timeScale = 1;
        EventBus.Instance.PauseGame();
        EventBus.Instance.PodePausar(true);

        dialogueIsPlaying = false;
        dialogueUI.SetActive(false);
        dialogueText.text = "";

        playerUI.SetActive(true);

    }

    private void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            dialogueText.text = currentStory.Continue();

            HandleTags(currentStory.currentTags);

        }
        else
        {
            ExitDialogueMode();
        }
    }

    private void HandleTags(List<string> currentTags)
    {

        foreach(string tag in currentTags)
        {
            string[] splitTag = tag.Split(':');
            if(splitTag.Length != 2)
            {
                Debug.LogError("Deu ruim");
            }

            string tagKey = splitTag[0].Trim();
            string tagValue = splitTag[1].Trim();

            switch(tagKey)
            {
                case SPEAKER_TAG:
                    displayNameText.text = tagValue;
                    break;

                case ANIM_TAG:
                    portraitAnimator.Play(tagValue);
                    break;

                case TP_TAG:
                    EventBus.Instance.GameOver();
                    
                    break;

                default:
                    Debug.Log("Tag errada");
                    break;
            }
        }

    }
}
