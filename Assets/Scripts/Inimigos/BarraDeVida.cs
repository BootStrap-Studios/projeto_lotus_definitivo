using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarraDeVida : MonoBehaviour
{
    [SerializeField] private Image barraDeVida;
    [SerializeField] private float velAnim;
    private float vidaAtualizada;
    private Camera myCamera;
    // Start is called before the first frame update
    void Start()
    {
        myCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.rotation = Quaternion.LookRotation(transform.position - myCamera.transform.position);

        barraDeVida.fillAmount = Mathf.MoveTowards(barraDeVida.fillAmount, vidaAtualizada, velAnim * Time.deltaTime);
    }

    public void AlterarBarraDeVida(float vidaAtual, float VidaMaxima)
    {
        vidaAtualizada = vidaAtual / VidaMaxima;
    }
}
