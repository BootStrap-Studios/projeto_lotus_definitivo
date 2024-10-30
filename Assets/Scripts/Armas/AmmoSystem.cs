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
    [SerializeField] private TextMeshProUGUI municaoUI; 
    [SerializeField] private Slider barraMunicaoUI; 
    [SerializeField] private Image barraBackground; 
    [SerializeField] private Image barraPreenchimento; 
    [SerializeField] private RawImage areaCerta;
    [SerializeField] private float velAnim;
    [SerializeField] private float velReloadBarra;
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
        AtualizandoUI();
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

            timer = timerReload;
        } 
        else
        {
            toNoReloadFull = false;

            barraBackground.color = new Color(0.4433962f, 0.4007013f, 0.3199982f, 1);
            barraPreenchimento.color = new Color(0.9811321f, 0.8757644f, 0.5414739f, 1);
        }
        
    }

    private void AtualizandoUI()
    {
        municaoUI.text = municaoAtual.ToString() + " / " + municaoTotal.ToString();
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
                barraBackground.color = new Color(0.27f, 0, 0, 1);
                barraPreenchimento.color = new Color(0.4622642f, 0, 0, 1);
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

        float valorMin = areaCerta.rectTransform.position.x - areaCerta.rectTransform.rect.width / 2;
        float valorMax = areaCerta.rectTransform.position.x + areaCerta.rectTransform.rect.width / 2;

        //Debug.Log(valorMin);
        //Debug.Log(barraMunicaoUI.handleRect.position.x);
        //Debug.Log(valorMax);

        if (barraMunicaoUI.handleRect.position.x > valorMin && barraMunicaoUI.handleRect.position.x < valorMax)
        {
            municaoAtual = municaoTotal;      
            toNoReloadFull = false;
            barraMunicaoUI.value = 1f;
            //barraMunicaoUI.gameObject.SetActive(false);
        }
        else
        {
            barraBackground.color = new Color(0.27f, 0, 0, 1);
            barraPreenchimento.color = new Color(0.4622642f, 0, 0, 1);
        }    
    }

    private void QTEreload()
    {
        //barraMunicaoUI.gameObject.SetActive(true);

        float posX = Random.Range(-67.3f, 47.4f);

        areaCerta.rectTransform.anchoredPosition = new Vector2(posX, areaCerta.rectTransform.anchoredPosition.y);
        areaCerta.enabled = true;
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

}
