using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : MonoBehaviour
{
    [SerializeField] private Transform pontaDaArma;

    [SerializeField] private int numeroDeProjeteis;

    [SerializeField] private float range;

    [SerializeField] private GameObject vfxTiro;

    [SerializeField] private AmmoSystem ammoSystem;

    [SerializeField] private int bulletsPerTap;

    [SerializeField] private LayerMask aimColliderMask;

    public string tipoDoBuff;
    public string tipoDeArma;


    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            ApertarGatilho();
            
        }
    }

    private void ApertarGatilho()
    {
        if(ammoSystem.municaoAtual >= bulletsPerTap && !ammoSystem.toNoReloadFull) {

            ammoSystem.GastandoMunicao(bulletsPerTap);

            AudioManager.instance.PlayOneShot(FMODEvents.instance.manoEsco, transform.position);

            for (int i = 0; i < numeroDeProjeteis; i++)
                {
                    ShotgunRay();
                    Debug.Log("Atirei");
                }

            

        }
    }

    private void ShotgunRay()
    {
        Vector3 direction = Camera.main.transform.forward;
        Vector3 spread = Vector3.zero;

        spread += Camera.main.transform.up * Random.Range(-.2f, .2f);
        spread += Camera.main.transform.right * Random.Range(-.2f, .2f);

        direction += spread.normalized * Random.Range(0f, 0.2f);

        RaycastHit hit;
        if(Physics.Raycast(Camera.main.transform.position, direction, out hit, range, aimColliderMask))
        {
            Debug.DrawLine(Camera.main.transform.position, hit.point, Color.green, 5f);

            if(hit.transform.CompareTag("Inimigo"))
            {
                hit.transform.GetComponent<Inimigo>().TomarDano(tipoDeArma, tipoDoBuff);
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
