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

    private RaycastHit[] hits;

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
        hits = Physics.BoxCastAll(pontaDaArma.position + boxOffSet, tamanhoDaCaixa / 2, pontaDaArma.forward, Quaternion.identity, Mathf.Infinity, ~layer);

        foreach(RaycastHit hit in hits)
        {
            if(hit.collider.CompareTag("Inimigo"))
            {
                hit.transform.GetComponent<Inimigo>().TomarDano(3f);
                Instantiate(vfxTiro, hit.point, Quaternion.identity);
               
                Debug.Log("Hitei Inimigo");

            } else
            {
                Instantiate(vfxTiro2, hit.point, Quaternion.identity);
                Debug.Log("Hitei");
                break;
            }
        }
    }

    private IEnumerator TiroShuriken()
    {
        yield return new WaitForSeconds(1.5f);

        ShurikenRay();
        ammoSystem.GastandoMunicao(bulletsPerTap);
    }
}
