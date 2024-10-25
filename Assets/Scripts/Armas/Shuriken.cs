using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuriken : MonoBehaviour
{
    [SerializeField] private AmmoSystem ammoSystem;

    [SerializeField] private GameObject vfxTiro;
    [SerializeField] private GameObject vfxTiro2;

    [SerializeField] private int bulletsPerTap;

    [SerializeField] private Transform pontaDaArma;
    [SerializeField] private Vector3 boxOffSet;

    [SerializeField] private Vector3 tamanhoDaCaixa;

    [SerializeField] private LayerMask layer;

    [SerializeField] private GameObject cubo;

    private RaycastHit[] hits;

    private float distanciaShuriken;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            ApertarGatilho();

        }
    }

    private void ApertarGatilho()
    {
        if (ammoSystem.municaoAtual >= bulletsPerTap && !ammoSystem.toNoReloadFull)
        {
            ShurikenRay();
            ammoSystem.GastandoMunicao(bulletsPerTap);
        }

    }

    private void ShurikenRay()
    {
        DetectandoParede();

        hits = Physics.BoxCastAll(pontaDaArma.position + boxOffSet, tamanhoDaCaixa / 2, pontaDaArma.forward, Quaternion.identity, distanciaShuriken, ~layer);

        cubo.transform.position = pontaDaArma.position + boxOffSet;
        cubo.transform.localScale = new Vector3(4f / 2, .5f / 2, 1000f);

        Vector3 newRotation = new Vector3(pontaDaArma.eulerAngles.x, pontaDaArma.eulerAngles.y, pontaDaArma.eulerAngles.z);
        cubo.transform.eulerAngles = newRotation;
        


        foreach(RaycastHit hit in hits)
        {
            Debug.Log(hit.collider.gameObject.name);
            if(hit.collider.CompareTag("Inimigo"))
            {
                hit.transform.GetComponent<Inimigo>().TomarDano(3f);
                Instantiate(vfxTiro, hit.point, Quaternion.identity);
               
                

            } else
            {
                Instantiate(vfxTiro2, hit.point, Quaternion.identity);
                
                
            }
        }
    }

    private void DetectandoParede()
    {
        RaycastHit hit;

        if(Physics.Raycast(pontaDaArma.position, pontaDaArma.forward, out hit, Mathf.Infinity))
        {
            if(!hit.collider.CompareTag("Inimigo")) {

                distanciaShuriken = hit.distance;

            }
        } else
        {
            distanciaShuriken = Mathf.Infinity;
        }
    }

    private IEnumerator TiroShuriken()
    {
        yield return new WaitForSeconds(1.5f);

        ShurikenRay();
        ammoSystem.GastandoMunicao(bulletsPerTap);
    }
}
