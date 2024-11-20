using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuPrincipal : MonoBehaviour
{
    [SerializeField] private GameObject menuBase_UI;
    [SerializeField] private GameObject contorles_UI;
    [SerializeField] private GameObject jogar_UI;
    [SerializeField] private Button bttnNovoJogo;
    [SerializeField] private Button bttnContinuarJogo;
    [SerializeField] private Button bttnVoltarJogo;

    public void BTNJogar()
    {
        menuBase_UI.SetActive(false);
        contorles_UI.SetActive(false);
        jogar_UI.SetActive(true);

        SaveSystemManager.instance.CarregarJogo();

        if (!SaveSystemManager.instance.TemSave())
        {

            bttnContinuarJogo.interactable = false;
        }

        //SceneManager.LoadScene("CutsceneInicial");
    }

    public void BTNControles()
    {
        menuBase_UI.SetActive(false);
        contorles_UI.SetActive(true);
        jogar_UI.SetActive(false);
    }

    public void BTNVoltar()
    {
        menuBase_UI.SetActive(true);
        contorles_UI.SetActive(false);
        jogar_UI.SetActive(false);
    }

    public void BTNSair()
    {
        Debug.Log("Fechar o jogo");
        Application.Quit();
    }

    public void BotaoContinuar()
    {
        SceneManager.LoadScene("Implemenetacao");
    }

    public void BTNNovoJogo()
    {
        DesabilitarBTNS();

        SaveSystemManager.instance.NovoJogo();
        SceneManager.LoadSceneAsync("CutsceneInicial");
    }
    public void BTNContinuarJogo()
    {
        DesabilitarBTNS();

        SceneManager.LoadSceneAsync("Implemenetacao");
    }

    private void DesabilitarBTNS()
    {
        bttnNovoJogo.interactable = false;
        bttnContinuarJogo.interactable = false;
        bttnVoltarJogo.interactable = false;
    }
}
