using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : MonoBehaviour
{
    [SerializeField] private Transform pontaDaArma;

    [SerializeField] private int numeroDeProjeteis;

    [SerializeField] private float range;

    [SerializeField] private GameObject vfxTiro;


    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            for(int i = 0; i < numeroDeProjeteis; i++)
            {
                ShotgunRay();
                Debug.Log("Atirei");
            }
            
        }
    }

    private void ShotgunRay()
    {
        Vector3 direction = pontaDaArma.forward;
        Vector3 spread = Vector3.zero;

        spread += pontaDaArma.up * Random.Range(-1f, 1f);
        spread += pontaDaArma.right * Random.Range(-1f, 1f);

        direction += spread.normalized * Random.Range(0f, 0.2f);

        RaycastHit hit;
        if(Physics.Raycast(pontaDaArma.transform.position, direction, out hit, range))
        {
            Debug.DrawLine(pontaDaArma.transform.position, hit.point, Color.green, 5f);

            if(hit.transform.CompareTag("Inimigo"))
            {
                hit.transform.GetComponent<Inimigo>().TomarDano(1f);
                Debug.Log("Hitei");
                
            }

            Instantiate(vfxTiro, hit.point, Quaternion.identity);
        } 
        else
        {
            Debug.DrawLine(pontaDaArma.transform.position, hit.point, Color.red, 5f);
        }


    }

   
}
