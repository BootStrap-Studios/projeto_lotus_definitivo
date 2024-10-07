using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VidaPlayer : MonoBehaviour
{
    [SerializeField] private GameObject playerUI;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private TextMeshProUGUI vidaUI;
    [SerializeField] private Slider barraDeVida;
    [SerializeField] private float velAnim;
    [SerializeField] private float vida;

    private float vidaAtual;
    private float vidaAtualizada;

    // Start is called before the first frame update
    private void Start()
    {
        vidaAtual = vida;
        AlterarBarraDeVida(vidaAtual, vida);
        vidaUI.text = vidaAtual.ToString() + " / " + vida.ToString();
    }

    private void Update()
    {
        barraDeVida.value = Mathf.MoveTowards(barraDeVida.value, vidaAtualizada, velAnim * Time.deltaTime);
    }

    public void TomarDano(float dano)
    {
        vidaAtual -= dano;
        AlterarBarraDeVida(vidaAtual, vida);
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
        AlterarBarraDeVida(vidaAtual, vida);
        vidaUI.text = vidaAtual.ToString() + " / " + vida.ToString();

        gameObject.transform.position = new Vector3(20f, 1f, -15f);
        gameObject.SetActive(true);
    }

    public void AlterarBarraDeVida(float vidaAtual, float VidaMaxima)
    {
        vidaAtualizada = vidaAtual / VidaMaxima;
    }
}
