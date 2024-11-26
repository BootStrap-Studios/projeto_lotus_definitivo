using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using FMODUnity;

public class MenuSystem : MonoBehaviour , ISave
{
    [Header("Menus")]
    [SerializeField] private GameObject playerUI;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject menuPauseUI;
    [SerializeField] private GameObject menuConfigUI;
    [SerializeField] private GameObject menuAudioUI;

    [Header("Congigurações")]
    [SerializeField] private Slider sensiOlhar;
    [SerializeField] private Slider sensiMirar;
    [SerializeField] private Scrollbar scrollbar;
    
    [Header("Outros")]
    [SerializeField] private MargelaH_CAM scriptCAM;
    [SerializeField] private PlayerMovement player;
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (stateMenu == State.menuFechado)
            {
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
                Debug.Log("Função indisponível agora!!!");
            }
        }
    }

    private void PodePausar(bool podePausar)
    {
        interagir = podePausar;

        if (!interagir)
        {
            stateMenu = State.menuInteragindo;
        }
        else
        {
            menuPauseUI.SetActive(false);
            stateMenu = State.menuFechado;
        }
    }

    public void PauseMenu()
    {        
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
        Time.timeScale = 0;

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
        Time.timeScale = 1;
        SceneManager.LoadScene("MenuPrincipal");
    }

    public void BTNSairJogo()
    {
        Debug.Log("Fechar o jogo");
        Application.Quit();
    }

    public void BTNTentarNovamente()
    {
        //EventBus.Instance.TP(false, true);
        //EventBus.Instance.PodePausar(false);

        //EventBus.Instance.FadeIn(0.5f, null, 5);
        //gameOverUI.SetActive(false);

        SceneManager.LoadScene("Implemenetacao");
        //player.transform.position = player.posInstituto;

        Time.timeScale = 1;

        //stateMenu = State.menuFechado;

        //EventBus.Instance.TP(false, true);
        //EventBus.Instance.PodePausar(true);
        //EventBus.Instance.PauseGame();

        //EventBus.Instance.FadeOut(1f, null);
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
