using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashCollider : MonoBehaviour
{
    private StatusJogador statusJogador;

    [SerializeField] private float danoDoDash;

    private void Start()
    {
        statusJogador = FindObjectOfType<StatusJogador>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Inimigo"))
        {
            if(statusJogador.dashAcertoCritico)
            {
                //Efeito do dash critico
            }

            if(statusJogador.dashBurst)
            {
                other.GetComponent<Inimigo>().TomarDanoDireto(danoDoDash);
            }

        }
    }
}
