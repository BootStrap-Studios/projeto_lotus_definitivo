using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using FMODUnity;
using Cinemachine;

public class MenuSystem : MonoBehaviour , ISave
{
    [Header("Menus")]
    [SerializeField] private GameObject playerUI;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject menuPauseUI;
    [SerializeField] private GameObject menuConfigUI;
    [SerializeField] private GameObject menuAudioUI;
    [SerializeField] private Button botaoSalvar;
    public bool podeSalvar;

    [Header("Congigurações")]
    [SerializeField] private Slider sensiOlhar;
    [SerializeField] private Slider sensiMirar;
    [SerializeField] private Scrollbar scrollbar;
    
    [Header("Outros")]
    [SerializeField] private MargelaH_CAM scriptCAM;
    [SerializeField] private PlayerMovement player;
    [SerializeField] private StatusJogador playerStatus;
    [SerializeField] private GameObject dialogo;
    private bool interagir;

    private StudioEventEmitter menuSnapshot;
    

    private enum State
    {
        menuFechado,
        menuPause,
        menuConfig,
        menuGameOver,
        menuAudio,
        menuInteragindo,
    };

    private State stateMenu;


    private void OnEnable()
    {
        EventBus.Instance.onGameOver += GameOverUI;
        EventBus.Instance.onPodePausar += PodePausar;
    }

    private void OnDisable()
    {
        EventBus.Instance.onGameOver -= GameOverUI;
        EventBus.Instance.onPodePausar -= PodePausar;
    }

    private void Start()
    {
        stateMenu = State.menuFechado;

        menuSnapshot = AudioManager.instance.InitializeEventEmitter(FMODEvents.instance.menuSnapshot, this.gameObject);
        menuSnapshot.Stop();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && interagir)
        {
            if (stateMenu == State.menuFechado)
            {
                //Debug.Log("Sistema de Menu: Abrindo meun...");
                PauseMenu();
                EventBus.Instance.PauseGame();
            }
            else if (stateMenu == State.menuPause)
            {
                DesligarUI();
            }
            else if (stateMenu == State.menuConfig)
            {
                PauseMenu();
            }
            else if (stateMenu == State.menuAudio)
            {
                PauseMenu();
            }
            else if (stateMenu == State.menuGameOver)
            {
                Debug.Log("Sistema de Menu: Função indisponível agora!!!");
            }
        }
    }

    private void PodePausar(bool podePausar)
    {
        interagir = podePausar;
    }

    public void PauseMenu()
    {   
        /*
        if (podeSalvar)
        {
            botaoSalvar.interactable = true;
        }
        else
        {
            botaoSalvar.interactable = false;
        }*/

        playerUI.SetActive(false);
        menuPauseUI.SetActive(true);
        menuConfigUI.SetActive(false);
        menuAudioUI.SetActive(false);

        AudioManager.instance.PlayOneShot(FMODEvents.instance.pauseGame, transform.position);
        menuSnapshot.Play();


        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        scriptCAM.AplicarSensi(sensiOlhar.value, sensiMirar.value);

        stateMenu = State.menuPause;

        Time.timeScale = 0;
    }    

    public void DesligarUI()    
    {
        EventBus.Instance.PauseGame();

        playerUI.SetActive(true);
        menuPauseUI.SetActive(false);     
        menuConfigUI.SetActive(false);
        menuAudioUI.SetActive(false);
        gameOverUI.SetActive(false);

        menuSnapshot.Stop();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        stateMenu = State.menuFechado;

        Time.timeScale = 1;
    }

    private void GameOverUI()
    {
        EventBus.Instance.PauseGame();

        playerUI.SetActive(false);
        gameOverUI.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        stateMenu = State.menuGameOver;
    }

    #region FunçõesBotõesUI

    public void BTNConfigMenu()
    {
        menuPauseUI.SetActive(false);
        menuConfigUI.SetActive(true);

        scrollbar.value = 0.999f;

        stateMenu = State.menuConfig;
    }

    public void BTNAudioMenu()
    {
        menuPauseUI.SetActive(false);
        menuAudioUI.SetActive(true);

        stateMenu = State.menuConfig;
    }

    public void BTNVoltarMenuInicial()
    {
        stateMenu = State.menuFechado;

        Time.timeScale = 1;
        SceneManager.LoadScene("MenuPrincipal");
    }

    public void BTNSairJogo()
    {
        Debug.Log("Sistema de Menu: Fechar o jogo");
        Application.Quit();
    }

    public void BTNTentarNovamente()
    {
        EventBus.Instance.FadeIn(2f, DesligarUI, 4f);

        EventBus.Instance.TP(false, true);

        player.transform.position = player.posInstituto;  
        scriptCAM.normalCAM.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.Value = 0;
        scriptCAM.normalCAM.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.Value = 180;

        EventBus.Instance.TP(true, false);
        
        podeSalvar = true;          

        playerStatus.Reset();

        if (dialogo != null)
        {
            StartCoroutine(AtivaDialogo());
        }
    }

    private IEnumerator AtivaDialogo()
    {
        yield return new WaitForSeconds(7.5f);

        dialogo.SetActive(true);
    }

    #endregion

    #region Save&Load

    public void CarregarSave(InfosSave save)
    {
        sensiOlhar.value = save.sensiOlhando;
        sensiMirar.value = save.sensiMirando;
    }

    public void SalvarSave(ref InfosSave save)
    {
        save.sensiOlhando = sensiOlhar.value;
        save.sensiMirando = sensiMirar.value;
    }

    #endregion
}
