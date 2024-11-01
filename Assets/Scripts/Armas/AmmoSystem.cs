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
    [SerializeField] private float timerQTECertoReload;
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
    private float valorBarraReload;
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

        //se o quick time event estiver ativado quando o player pressionar o "R", a função QTECheck é chamada
        if (Input.GetKeyDown(KeyCode.R) && QTE)
        {

            QTEcheck();
        }

        BarraReloadMovimento();
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
        
        //Se a munição chegar a zero, o player tem que esperar a arma carregar totalmente, portando a boolando fica true e o QTE é ativado
        if(municaoAtual == 0)
        {
            timer = timerAux;
            toNoReloadFull = true;

            QTE = true;
            QTEreload();            
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
        }
        
    }

    private void BarraReloadMovimento()
    {
        //função que faz a barra se mexer para o QTE do reload

        if (toNoReloadFull && QTE)
        {
            valorBarraReload += Time.deltaTime;

            barraMunicaoUI.value = Mathf.MoveTowards(barraMunicaoUI.value, valorBarraReload, velReloadBarra * Time.deltaTime);

            if(barraMunicaoUI.value >= 1f)
            {
                QTE = false;
                areaCerta.enabled = false;
                areaCertaMeio.enabled = false;
                tracoCerto.enabled = false;
                barraBackground.enabled = false;
            }
        }
    }

    private void QTEcheck()
    {
        //função que verifica se o player acertou ou não o QTE

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

            barraBackground.enabled = false;
        }    
    }

    private void QTEreload()
    {
        //função que ativa a barra do QTE e aleatoriza a posição que o player deve acertar

       barraBackground.enabled = true;

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
        barraBackground.enabled = false;

        statusJogador.ReloadBuffs();

        for (int i = 0; i < balas.Length; i++)
        {
            balas[i].color = new Color(1, 1, 1, 1);

            yield return new WaitForSeconds(timerQTECertoReload);
        }

        municaoAtual = municaoTotal;
        toNoReloadFull = false;
        barraMunicaoUI.value = 0f;
    }

}
