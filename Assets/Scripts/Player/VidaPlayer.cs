using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VidaPlayer : MonoBehaviour
{
    [SerializeField] private GameObject playerUI;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private TextMeshProUGUI vidaUI;
    [SerializeField] private BarraDeVida _barraDeVida;
    [SerializeField] private float vida;
    private float vidaAtual;

    // Start is called before the first frame update
    void Start()
    {
        vidaAtual = vida;
        _barraDeVida.AlterarBarraDeVida(vidaAtual, vida);
        vidaUI.text = vidaAtual.ToString() + " / " + vida.ToString();
    }

    public void TomarDano(float dano)
    {
        vidaAtual -= dano;
        _barraDeVida.AlterarBarraDeVida(vidaAtual, vida);
        vidaUI.text = vidaAtual.ToString() + " / " + vida.ToString();

        if (vidaAtual <= 0)
        {
            playerUI.SetActive(false);
            gameOverUI.SetActive(true);
            gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void TentarNovamente()
    {
        playerUI.SetActive(true);
        gameOverUI.SetActive(false);
        
        Cursor.lockState = CursorLockMode.Locked;

        vidaAtual = vida;
        _barraDeVida.AlterarBarraDeVida(vidaAtual, vida);
        vidaUI.text = vidaAtual.ToString() + " / " + vida.ToString();

        gameObject.transform.position = new Vector3(20f, 1f, -15f);
        gameObject.SetActive(true);
    }
}
