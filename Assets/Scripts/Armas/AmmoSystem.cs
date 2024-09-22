using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class AmmoSystem : MonoBehaviour
{
    [SerializeField] private int municaoTotal;
    public int municaoAtual;

    [SerializeField] private float timerAux;
    [SerializeField] private float timerReload;
    private float timer;

    [SerializeField] private TextMeshProUGUI municaoUI;

    public bool toNoReloadFull;


    void Start()
    {
        //Setando as variáveis

        toNoReloadFull = false;
        timer = timerAux;
        municaoAtual = municaoTotal;
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
        } else
        {
            toNoReloadFull = false;
        }
        
    }

    private void AtualizandoUI()
    {
        municaoUI.text = municaoAtual.ToString() + " / " + municaoTotal.ToString();
    }

}
