    using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arma : MonoBehaviour
{
    [SerializeField] private LayerMask aimColliderMask;

    [SerializeField] private ObjectPool objectPool;

    [SerializeField] private Transform pontaDaArma;

    [SerializeField] private AmmoSystem ammoSystem;
    [SerializeField] private int bulletsPerTap;

    [SerializeField] private GameObject vfxHit;

    private Projetil projetil;
    Transform hitTransform;
    Vector3 hitPoint;

    public string tipoDoBuff;
    public string tipoDeArma;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            ApertarGatilho();
        }
    }


    private void ApertarGatilho()
    {
        //Checa se o player tem munição o bastante para atirar e se ele não está no reload full
        if(ammoSystem.municaoAtual >= bulletsPerTap && !ammoSystem.toNoReloadFull)
        {
            ammoSystem.GastandoMunicao(bulletsPerTap);
            Raycast();
            AudioManager.instance.PlayOneShot(FMODEvents.instance.manoPistol, transform.position);
            //PedirProjetil();

        } else
        {
            Debug.Log("Não tem munição, ou to reloadando");
        }
        

    }

    private void PedirProjetil()
    {
        projetil = objectPool.GetPooledObject().GetComponent<Projetil>();
    }

    private void Raycast()
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderMask))
        {
            hitPoint = raycastHit.point;
            hitTransform = raycastHit.transform;

            Atirando();
        }
    }

    private void Atirando()
    {
        if(hitTransform != null)
        {
            if(hitTransform.CompareTag("Inimigo")) {

                try
                {
                    hitTransform.GetComponent<Inimigo>().TomarDano(tipoDeArma, tipoDoBuff);
                    Instantiate(vfxHit, hitPoint, Quaternion.identity);
                }
                catch
                {
                    Instantiate(vfxHit, hitPoint, Quaternion.identity);
                }

            } else
            {
                Instantiate(vfxHit, hitPoint, Quaternion.identity);
            }
        } 
     


        //projetil.transform.position = pontaDaArma.position;
        //projetil.gameObject.SetActive(true);
        //projetil.DefineTargetPosition(hitTransform);
    }



}
