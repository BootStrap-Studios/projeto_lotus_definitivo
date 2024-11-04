using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderArea : MonoBehaviour
{
    private string oqueFazer;
    private float tempoAtivo;
    private float tempoAtivoAux = 0.5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Inimigo"))
        {
            if (oqueFazer == "darDanoExplosao")
            {
                other.GetComponent<Inimigo>().TomarDanoDireto(3f);
            }
            else if (oqueFazer == "vulneravel")
            {
                other.GetComponent<Inimigo>().vulneravel = true;
            }
        }
    }

    public void OqueFazer(string _oqueFazer)
    {
        oqueFazer = _oqueFazer;
        tempoAtivo = tempoAtivoAux;
    }

    private void Update()
    {
        if(tempoAtivo > 0)
        {
            tempoAtivo -= Time.deltaTime;
        }
        else if(tempoAtivo <= 0)
        {
            tempoAtivo = tempoAtivoAux;
            gameObject.SetActive(false);
        }
    }
}
