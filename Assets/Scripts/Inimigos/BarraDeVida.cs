using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarraDeVida : MonoBehaviour
{
    [SerializeField] private Slider barraDeVida;
    [SerializeField] private float velAnim;
    private float vidaAtualizada;
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
    }
}
