using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarraDeVida : MonoBehaviour
{
    [SerializeField] Image barraDeVida;
    [SerializeField] float velAnim;
    float vidaAtualizada = 1;
    Camera myCamera;
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

    public void AlterarBarraDeVida(int vidaAtual, int VidaMaxima)
    {
       vidaAtualizada = (float) vidaAtual / VidaMaxima;
    }
}
