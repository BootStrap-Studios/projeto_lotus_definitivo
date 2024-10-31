using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class AmmoSystem : MonoBehaviour
{
    [Header("Munição")]
    [SerializeField] private int municaoTotal;
    public int municaoAtual;
    public bool toNoReloadFull;

    [Header("Timers")]
    [SerializeField] private float timerAux;
    [SerializeField] private float timerReload;
    private float timer;

    [Header("UI")]
    [SerializeField] private Slider barraMunicaoUI; 
    [SerializeField] private Image barraBackground; 
    [SerializeField] private Image tracoCerto; 
    [SerializeField] private Image areaCerta;
    [SerializeField] private Image areaCertaMeio;
    [SerializeField] private float velAnim;
    [SerializeField] private float velReloadBarra;
    [SerializeField] private Image[] balas;
    private float municaoAtualizada;
    private bool QTE;

    private StatusJogador statusJogador;


    void Start()
    {
        //Setando as variáveis

        toNoReloadFull = false;
        timer = timerAux;
        municaoAtual = municaoTotal;

        statusJogador = FindObjectOfType<StatusJogador>();

    }
    void Update()
    {
        //todo segundo um timer é tickado, para saber se a arma deve ou não recarregar.

        timer -= Time.deltaTime;


        //Se o timer chegar a zero, a função de reload é chamada.
        if(timer <= 0f)
        {
            Reload();
        }


        //Atualizando a quantidade de balas na ui
        AtualizaBarraReload();

        if (Input.GetKeyDown(KeyCode.R) && QTE)
        {
            QTEcheck();
        }
    }

    public void GastandoMunicao(int municao)
    {
        //Essa função é chamada do scrip da arma sempre que o player atira, gastando a munição necessária

        municaoAtual -= municao;

        for(int i = 0; i < balas.Length; i++)
        {
            if (i >= municaoAtual)
            {
                balas[i].color = new Color(0.6f, 0.6f, 0.6f, 1);
            }
        }
        
        //Se a munição chegar a zero, o player tem que esperar a arma carregar totalmente, portando a boolando fica true
        if(municaoAtual == 0)
        {
            timer = timerAux;
            toNoReloadFull = true;

            QTE = true;
            QTEreload();
            municaoAtualizada = 0f;
        }
        else {

            timer = timerAux;
        }

        
    }

    private void Reload()
    {
        //A função de reload recarrega 1 bala da arma sempre que é chamada, se a munição ficar cheia ela reseta a bool do reload full

        if(municaoAtual < municaoTotal)
        {
            municaoAtual++;

            for (int i = 0; i < balas.Length; i++)
            {
                if (i < municaoAtual)
                {
                    balas[i].color = new Color(1, 1, 1, 1);
                }
            }

            timer = timerReload;
        } 
        else
        {
            toNoReloadFull = false;

            barraMunicaoUI.gameObject.SetActive(false);
            barraBackground.color = new Color(0.85f, 0.85f, 0.85f, 1);
        }
        
    }

    private void AtualizaBarraReload()
    {
        if (toNoReloadFull && QTE)
        {
            municaoAtualizada += Time.deltaTime;

            barraMunicaoUI.value = Mathf.MoveTowards(barraMunicaoUI.value, municaoAtualizada, velReloadBarra * Time.deltaTime);

            if(barraMunicaoUI.value >= 1f)
            {
                QTE = false;
                areaCerta.enabled = false;
                areaCertaMeio.enabled = false;
                tracoCerto.enabled = false;
                barraBackground.color = new Color(0.6f, 0, 0, 1);
            }
        }
        else
        {
            municaoAtualizada = (float) municaoAtual / municaoTotal;

            barraMunicaoUI.value = Mathf.MoveTowards(barraMunicaoUI.value, municaoAtualizada, velAnim * Time.deltaTime);
        }
    }

    private void QTEcheck()
    {
        QTE = false;
        areaCerta.enabled = false;
        areaCertaMeio.enabled = false;
        tracoCerto.enabled = false;

        float valorMin = areaCerta.rectTransform.position.x - areaCerta.rectTransform.rect.width / 2;
        float valorMax = areaCerta.rectTransform.position.x + areaCerta.rectTransform.rect.width / 2;

        if (barraMunicaoUI.handleRect.position.x > valorMin && barraMunicaoUI.handleRect.position.x < valorMax)
        {
            StartCoroutine(BalasRecaregando());                  
        }
        else
        {
            for (int i = 0; i < balas.Length; i++)
            {
                balas[i].color = new Color(0.6f, 0, 0, 1);
            }
            barraBackground.color = new Color(0.6f, 0, 0, 1);
        }    
    }

    private void QTEreload()
    {
        barraMunicaoUI.gameObject.SetActive(true);

        float posX = Random.Range(-67.3f, 47.4f);

        areaCerta.rectTransform.anchoredPosition = new Vector2(posX, areaCerta.rectTransform.anchoredPosition.y);
        areaCertaMeio.rectTransform.anchoredPosition = new Vector2(posX, areaCertaMeio.rectTransform.anchoredPosition.y);
        areaCerta.enabled = true;
        areaCertaMeio.enabled = true;
        tracoCerto.enabled = true;
    }

    public void MunicaoInfinita()
    {
        StartCoroutine(MunicaoInfinitaCoroutine());
    }

    private IEnumerator MunicaoInfinitaCoroutine()
    {
        municaoAtual = 1000000000;

        yield return new WaitForSeconds(statusJogador.duracaoUltimateMovimentacao);

        municaoAtual = municaoTotal;
    }

    private IEnumerator BalasRecaregando()
    {
        barraMunicaoUI.gameObject.SetActive(false);

        for (int i = 0; i < balas.Length; i++)
        {
            balas[i].color = new Color(1, 1, 1, 1);

            yield return new WaitForSeconds(0.1f);
        }

        municaoAtual = municaoTotal;
        toNoReloadFull = false;
        barraMunicaoUI.value = 1f;
    }

}
