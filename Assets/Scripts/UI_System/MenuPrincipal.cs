using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuPrincipal : MonoBehaviour, ISave
{
    [SerializeField] private GameObject menuBase_UI;
    [SerializeField] private GameObject contorles_UI;
    [SerializeField] private GameObject jogar_UI;
    [SerializeField] private GameObject audio_UI;
    [SerializeField] private Button bttnNovoJogo;
    [SerializeField] private Button bttnContinuarJogo;
    [SerializeField] private Button bttnVoltarJogo;
    [SerializeField] private Slider sensiOlhar;
    [SerializeField] private Slider sensiMirar;

    public void BTNJogar()
    {
        menuBase_UI.SetActive(false);
        contorles_UI.SetActive(false);
        jogar_UI.SetActive(true);
        audio_UI.SetActive(false);

        SaveSystemManager.instance.CarregarJogo();

        if (SaveSystemManager.instance.TemSave())
        {
            bttnContinuarJogo.interactable = true;
        }
    }

    public void BTNControles()
    {
        menuBase_UI.SetActive(false);
        contorles_UI.SetActive(true);
        jogar_UI.SetActive(false);
        audio_UI.SetActive(false);
    }

    public void BTNVoltar()
    {
        menuBase_UI.SetActive(true);
        contorles_UI.SetActive(false);
        jogar_UI.SetActive(false);
        audio_UI.SetActive(false);
    }

    public void BTNSair()
    {
        Debug.Log("Fechar o jogo");
        Application.Quit();
    }

    public void BTNNovoJogo()
    {
        DesabilitarBTNS();
        AudioManager.instance.CleanUp();

        SaveSystemManager.instance.NovoJogo();
        //SceneManager.LoadScene("MargelaH");
        //SceneManager.LoadSceneAsync("CutsceneInicial");
        SceneManager.LoadSceneAsync("Implemenetacao");
    }
    public void BTNContinuarJogo()
    {
        DesabilitarBTNS();

        //SceneManager.LoadScene("MargelaH");
        SceneManager.LoadSceneAsync("Implemenetacao");
    }

    public void BTNAudioConfig()
    {
        menuBase_UI.SetActive(false);
        contorles_UI.SetActive(false);
        jogar_UI.SetActive(false);
        audio_UI.SetActive(true);
    }

    private void DesabilitarBTNS()
    {
        bttnNovoJogo.interactable = false;
        bttnContinuarJogo.interactable = false;
        bttnVoltarJogo.interactable = false;
    }

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
