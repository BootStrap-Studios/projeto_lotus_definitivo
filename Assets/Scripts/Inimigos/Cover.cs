using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Cover : MonoBehaviour
{
    public bool coverCheio;
    public bool coverEscondido;
    [SerializeField] private Material verde;
    [SerializeField] private Material vermelho;
    [SerializeField] private Material azul;

    private PlayerMovement player;
    public Inimigo inimigoAtual;


    private void Start()
    {
        player = FindObjectOfType<PlayerMovement>();
    }

    private void Update()
    {
        if(inimigoAtual == null)
        {
            VerificaCover2();
            coverCheio = false;
        }
        else
        {
            coverCheio = true;
            if (inimigoAtual.inimigoSimples)
            {
                inimigoAtual.stateInimigoSimples.coverSelecionado = this;
            }
            else if (inimigoAtual.inimigoSniper)
            {
                inimigoAtual.stateInimigoSniper.coverSelecionado = this;
            }
        }
    }

    /*private void VerificaCover()
    {
        Vector3 direcao = player.transform.position - gameObject.transform.position;
        float angulo = Vector3.Angle(direcao, gameObject.transform.forward);

        if (direcao.magnitude < 200f && angulo < 100f)
        {
            coverEscondido = false;
            coverCheio = false;

            if (inimigoAtual != null)
            {
                inimigoAtual.stateInimigo.coverSelecionado = null;
            }
            inimigoAtual = null;

            this.GetComponent<MeshRenderer>().material = vermelho;
        }
        else
        {
            coverEscondido = true;

            if (coverCheio)
            {
                this.GetComponent<MeshRenderer>().material = azul;
            }
            else
            {
                this.GetComponent<MeshRenderer>().material = verde;
            }
        }
    }*/
        
    public void VerificaCover2()
    {
        RaycastHit hit;
        Vector3 direcao = player.transform.position - gameObject.transform.position;
        float angulo = Vector3.Angle(direcao, gameObject.transform.forward);

        if (Physics.Linecast(gameObject.transform.position, player.transform.position, out hit))
        {
            if (hit.transform.tag == "Player" || direcao.magnitude < 1000f && angulo < 110f)
            {
                coverEscondido = false;
                coverCheio = false;

                if (inimigoAtual != null)
                {
                    if (inimigoAtual.inimigoSimples)
                    {
                        inimigoAtual.stateInimigoSimples.coverSelecionado = this;
                    }
                    else if (inimigoAtual.inimigoSniper)
                    {
                        inimigoAtual.stateInimigoSniper.coverSelecionado = this;
                    }
                }
                inimigoAtual = null;

                this.GetComponent<MeshRenderer>().material = vermelho;
            }
            else
            {
                coverEscondido = true;

                if (coverCheio)
                {
                    this.GetComponent<MeshRenderer>().material = azul;
                }
                else
                {
                    this.GetComponent<MeshRenderer>().material = verde;
                }
            }
        }
    }
}
