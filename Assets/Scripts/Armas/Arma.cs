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

    [SerializeField] private AudioSource sourceDisparo;

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
        //Checa se o player tem muni��o o bastante para atirar e se ele n�o est� no reload full
        if(ammoSystem.municaoAtual >= bulletsPerTap && !ammoSystem.toNoReloadFull)
        {
            ammoSystem.GastandoMunicao(bulletsPerTap);
            Raycast();
            sourceDisparo.PlayOneShot(sourceDisparo.clip);
            //PedirProjetil();

        } else
        {
            Debug.Log("N�o tem muni��o, ou to reloadando");
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

                hitTransform.GetComponent<Inimigo>().TomarDano(tipoDeArma, tipoDoBuff);
                Instantiate(vfxHit, hitPoint, Quaternion.identity);

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
