using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransicaoDeMusica : MonoBehaviour
{
    public bool fabrica;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if (fabrica)
            {
                AudioManager.instance.SetMusicParameter("area", 3);
                AudioManager.instance.SetAmbienceParameter("Ambiente", 1f);
            }
            else
            {
                AudioManager.instance.SetMusicParameter("area", 0);
                AudioManager.instance.SetAmbienceParameter("Ambiente", 0f);
            }
        }
        
    }
}
