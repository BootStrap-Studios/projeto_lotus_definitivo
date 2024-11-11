using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    [SerializeField] private GameObject menuBase_UI;
    [SerializeField] private GameObject contorles_UI;

    public void BTNJogar()
    {
        SceneManager.LoadScene("CutsceneInicial");
    }

    public void BotaoContinuar()
    {
        SceneManager.LoadScene("Implemenetacao");
    }
    public void BTNControles()
    {
        menuBase_UI.SetActive(false);
        contorles_UI.SetActive(true);
    }

    public void BTNVoltar()
    {
        menuBase_UI.SetActive(true);
        contorles_UI.SetActive(false);
    }

    public void BTNSair()
    {
        Debug.Log("Fechar o jogo");
        Application.Quit();
    }
}
