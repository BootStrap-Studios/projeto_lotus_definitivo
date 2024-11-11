using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class AmmoSystem : MonoBehaviour
{
    [Header("Muni��o")]
    [SerializeField] public int municaoTotal;
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
    [SerializeField] private float velBrilhoBalas;
    [SerializeField] private Image[] balas;
    private float valorBarraReload;
    private bool QTE;

    private StatusJogador statusJogador;
    private bool brilhar;
    private float valorCor;
    private Color corNova;
    private int faseBrilho;

    [SerializeField] private AudioSource sourceReloadCerto;


    void Start()
    {
        //Setando as vari�veis

        toNoReloadFull = false;
        timer = timerAux;
        municaoAtual = municaoTotal;

        statusJogador = FindObjectOfType<StatusJogador>();

    }
    void Update()
    {
        //todo segundo um timer � tickado, para saber se a arma deve ou n�o recarregar.
        timer -= Time.deltaTime;

        //Se o timer chegar a zero, a fun��o de reload � chamada.
        if (timer <= 0f)
        {
            Reload();
        }

        //se o quick time event estiver ativado quando o player pressionar o "R", a fun��o QTECheck � chamada
        if (Input.GetKeyDown(KeyCode.Mouse0) && QTE)
        {

            QTEcheck();
        }

        BarraReloadMovimento();

        if (brilhar)
        {
            Brilhando();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            MunicaoInfinita();
        }
    }

    public void GastandoMunicao(int municao)
    {
        //Essa fun��o � chamada do scrip da arma sempre que o player atira, gastando a muni��o necess�ria

        municaoAtual -= municao;

        for (int i = 0; i < balas.Length; i++)
        {
            if (i >= municaoAtual)
            {
                balas[i].color = new Color(0.6f, 0.6f, 0.6f, 1);
            }
        }

        //Se a muni��o chegar a zero, o player tem que esperar a arma carregar totalmente, portando a boolando fica true e o QTE � ativado
        if (municaoAtual == 0)
        {
            timer = timerAux;
            toNoReloadFull = true;

            QTE = true;
            QTEreload();
        }
        else
        {

            timer = timerAux;
        }


    }

    private void Reload()
    {
        //A fun��o de reload recarrega 1 bala da arma sempre que � chamada, se a muni��o ficar cheia ela reseta a bool do reload full

        if (municaoAtual < municaoTotal && !toNoReloadFull)
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
    }

    private void BarraReloadMovimento()
    {
        //fun��o que faz a barra se mexer para o QTE do reload

        if (toNoReloadFull && QTE)
        {
            valorBarraReload += Time.deltaTime;

            barraMunicaoUI.value = Mathf.MoveTowards(barraMunicaoUI.value, valorBarraReload, velReloadBarra * Time.deltaTime);

            if (barraMunicaoUI.value >= 1f)
            {
                QTE = false;
                areaCerta.enabled = false;
                areaCertaMeio.enabled = false;
                tracoCerto.enabled = false;
                barraBackground.enabled = false;
                barraMunicaoUI.value = 0;

                for (int i = 0; i < balas.Length; i++)
                {
                    balas[i].color = new Color(0.6f, 0, 0, 1);
                }

                StartCoroutine(BalasRecaregando(timerReload));
            }
        }
    }

    private void QTEcheck()
    {
        //fun��o que verifica se o player acertou ou n�o o QTE

        QTE = false;
        areaCerta.enabled = false;
        areaCertaMeio.enabled = false;
        tracoCerto.enabled = false;

        float valorMin = areaCerta.rectTransform.position.x - areaCerta.rectTransform.rect.width / 2;
        float valorMax = areaCerta.rectTransform.position.x + areaCerta.rectTransform.rect.width / 2;

        if (barraMunicaoUI.handleRect.position.x > valorMin && barraMunicaoUI.handleRect.position.x < valorMax)
        {
            StartCoroutine(BalasRecaregando(timerQTECertoReload));
            statusJogador.ReloadBuffs();
            sourceReloadCerto.PlayOneShot(sourceReloadCerto.clip);
        }
        else
        {
            for (int i = 0; i < balas.Length; i++)
            {
                balas[i].color = new Color(0.6f, 0, 0, 1);
            }

            barraBackground.enabled = false;

            StartCoroutine(BalasRecaregando(timerReload));
        }
    }

    private void QTEreload()
    {
        //fun��o que ativa a barra do QTE e aleatoriza a posi��o que o player deve acertar

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
        brilhar = true;
        municaoAtual = 1000000000;

        yield return new WaitForSeconds(statusJogador.duracaoUltimateMovimentacao);

        brilhar = false;
        faseBrilho = 0;

        for (int i = 0; i < balas.Length; i++)
        {
            balas[i].color = new Color(1, 1, 1, 1);
        }

        municaoAtual = municaoTotal;
    }

    private IEnumerator BalasRecaregando(float tempoReload)
    {
        barraBackground.enabled = false;

        for (int i = 0; i < balas.Length; i++)
        {
            balas[i].color = new Color(1, 1, 1, 1);

            yield return new WaitForSeconds(tempoReload);
        }

        municaoAtual = municaoTotal;
        toNoReloadFull = false;
        barraMunicaoUI.value = 0f;
    }

    private void Brilhando()
    {
        if (faseBrilho == 0)
        {
            for (int i = 0; i < balas.Length; i++)
            {
                balas[i].color = new Color(1, 0, 0, 1);
            }

            faseBrilho = 1;
            valorCor = 0;
        }
        else if (faseBrilho == 1)
        {
            corNova = new Color(1, valorCor, 0, 1);

            valorCor += Time.deltaTime * velBrilhoBalas;

            if (valorCor >= 1)
            {
                faseBrilho = 2;
                valorCor = 1;
            }
        }
        else if (faseBrilho == 2)
        {
            corNova = new Color(valorCor, 1, 0, 1);

            valorCor -= Time.deltaTime * velBrilhoBalas;

            if (valorCor <= 0)
            {
                faseBrilho = 3;
                valorCor = 0;
            }
        }
        else if (faseBrilho == 3)
        {
            corNova = new Color(0, 1, valorCor, 1);

            valorCor += Time.deltaTime * velBrilhoBalas;

            if (valorCor >= 1)
            {
                faseBrilho = 4;
                valorCor = 1;
            }
        }
        else if (faseBrilho == 4)
        {
            corNova = new Color(0, valorCor, 1, 1);

            valorCor -= Time.deltaTime * velBrilhoBalas;

            if (valorCor <= 0)
            {
                faseBrilho = 5;
                valorCor = 0;
            }
        }
        else if (faseBrilho == 5)
        {
            corNova = new Color(valorCor, 0, 1, 1);

            valorCor += Time.deltaTime * velBrilhoBalas;

            if (valorCor >= 1)
            {
                faseBrilho = 6;
                valorCor = 1;
            }
        }
        else if (faseBrilho == 6)
        {
            corNova = new Color(corNova.r, corNova.g, valorCor, 1);

            valorCor -= Time.deltaTime * velBrilhoBalas;

            if (valorCor <= 0)
            {
                faseBrilho = 1;
                valorCor = 0;
            }
        }

        for (int i = 0; i < balas.Length; i++)
        {
            balas[i].color = corNova;
        }
    }

}