using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuSystem : MonoBehaviour
{
    [SerializeField] private GameObject playerUI;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject menuPauseUI;
    [SerializeField] private GameObject menuConfigUI;
    [SerializeField] private string nomeScene;

    private enum State
    {
        menuFechado,
        menuPause,
        menuConfig,
        menuGameOver,
    };

    private State stateMenu;


    private void OnEnable()
    {
        EventBus.Instance.onGameOver += GameOverUI;
    }

    private void OnDisable()
    {
        EventBus.Instance.onGameOver -= GameOverUI;
    }

    private void Start()
    {
        stateMenu = State.menuFechado;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(stateMenu == State.menuFechado)
            {
                PauseMenu();
                EventBus.Instance.PauseGame();
            }
            else if(stateMenu == State.menuPause)
            {
                DesligarUI();
            }
            else if(stateMenu == State.menuConfig)
            {
                PauseMenu();
            }
            else if(stateMenu == State.menuGameOver)
            {
                Debug.Log("Função indisponível agora!!!");
            }
        }
    }

    public void PauseMenu()
    {        
        playerUI.SetActive(false);
        menuPauseUI.SetActive(true);
        menuConfigUI.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        stateMenu = State.menuPause;

        Time.timeScale = 0;
    }    

    public void DesligarUI()    
    {
        EventBus.Instance.PauseGame();

        playerUI.SetActive(true);
        menuPauseUI.SetActive(false);     
        menuConfigUI.SetActive(false);
        gameOverUI.SetActive(false);

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

    //FUNÇÕES ATRELADAS SOMENTE A BOTÕES NA UI
    #region FunçõesBotõesUI

    public void BTNConfigMenu()
    {
        menuPauseUI.SetActive(false);
        menuConfigUI.SetActive(true);

        stateMenu = State.menuConfig;
    }


    public void BTNVoltarMenuInicial()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }

    public void BTNSairJogo()
    {
        Debug.Log("Fechar o jogo");
        Application.Quit();
    }

    public void BTNTentarNovamente()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(nomeScene);
    }

    #endregion
}
