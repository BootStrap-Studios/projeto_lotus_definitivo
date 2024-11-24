using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltCorrosao : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Inimigo"))
        {
            if (other.GetComponent<Inimigo>() != null)
            {
                Debug.Log("Corroendo");
                other.GetComponent<Inimigo>().CorrosaoDireto();
            }
        }
    }
}
