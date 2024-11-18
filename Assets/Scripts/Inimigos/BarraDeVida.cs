using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarraDeVida : MonoBehaviour
{
    [Header("Vida:")]
    [SerializeField] private Slider barraDeVida;
    [SerializeField] private float velAnim;
    [SerializeField] private Image vidaIMG;
    private float vidaAtualizada;
    private bool vidaBaixa;

    [Header("Status:")]
    [SerializeField] private Image burst;
    [SerializeField] private Image burstBG;
    [SerializeField] private Image movimentacao;
    [SerializeField] private Image movimentacaoBG;
    [SerializeField] private Image corrosao;
    [SerializeField] private Image corrosaoBG;
    [SerializeField] private Image vulneravel;
    [SerializeField] private Image vulneravelBG;
    [SerializeField] private Image fraco;
    [SerializeField] private Image fracoBG;

    private Camera myCamera;

    void Start()
    {
        myCamera = Camera.main;
    }

    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - myCamera.transform.position);

        barraDeVida.value = Mathf.MoveTowards(barraDeVida.value, vidaAtualizada, velAnim * Time.deltaTime);
    }

    public void AlterarBarraDeVida(float vidaAtual, float VidaMaxima)
    {
        vidaAtualizada = vidaAtual / VidaMaxima;

        if (vidaAtual <= VidaMaxima / 4 && !vidaBaixa)
        {
            vidaBaixa = true;
            StartCoroutine(CO_VidaPiscandoFadeOut());
        }
    }

    private IEnumerator CO_VidaPiscandoFadeIn()
    {
        while (vidaIMG.color.a < 1)
        {
            vidaIMG.color = new Color(vidaIMG.color.r, vidaIMG.color.g, vidaIMG.color.b, vidaIMG.color.a + (Time.deltaTime / 0.3f));
            yield return null;
        }

        yield return new WaitForSeconds(.3f);
        StartCoroutine(CO_VidaPiscandoFadeOut());
    }

    private IEnumerator CO_VidaPiscandoFadeOut()
    {
        while (vidaIMG.color.a > 0)
        {
            vidaIMG.color = new Color(vidaIMG.color.r, vidaIMG.color.g, vidaIMG.color.b, vidaIMG.color.a - (Time.deltaTime / 0.3f));
            yield return null;
        }

        StartCoroutine(CO_VidaPiscandoFadeIn());
    }

    public void AtualizaStatus(float statusAtual, float statusTotal, string qualStatus, bool status)
    {
        switch (qualStatus)
        {
            case "burst":

                if(statusAtual <= 0)
                {
                    burst.gameObject.SetActive(status);
                    burstBG.gameObject.SetActive(status);
                    burst.fillAmount = 0;
                }
                else
                {
                    burst.gameObject.SetActive(status);
                    burstBG.gameObject.SetActive(status);
                    burst.fillAmount = statusAtual / statusTotal;
                }

                break;

            case "movimentacao":

                if (statusAtual <= 0)
                {
                    movimentacao.gameObject.SetActive(status);
                    movimentacaoBG.gameObject.SetActive(status);
                    movimentacao.fillAmount = 0;
                }
                else
                {
                    movimentacao.gameObject.SetActive(status);
                    movimentacaoBG.gameObject.SetActive(status);
                    movimentacao.fillAmount = statusAtual / statusTotal;
                }

                break;

            case "corrosao":

                if (statusAtual <= 0)
                {
                    corrosao.gameObject.SetActive(status);
                    corrosaoBG.gameObject.SetActive(status);
                    corrosao.fillAmount = 0;
                }
                else
                {
                    corrosao.gameObject.SetActive(status);
                    corrosaoBG.gameObject.SetActive(status);
                    corrosao.fillAmount = statusAtual / statusTotal;
                }

                break;

            case "vulneravel":

                vulneravel.gameObject.SetActive(status);
                vulneravelBG.gameObject.SetActive(status);
                Debug.Log("vulneravel: " + status);

                break;

            case "fraco":

                fraco.gameObject.SetActive(status);
                fracoBG.gameObject.SetActive(status);

                break;
        }
          
    }
}
