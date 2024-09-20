using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arma : MonoBehaviour
{
    [SerializeField] private LayerMask aimColliderMask;

    [SerializeField] private ObjectPool objectPool;

    [SerializeField] private Transform pontaDaArma;

    private Projetil projetil;
    Vector3 hitTransform;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            ApertarGatilho();
        }
    }


    private void ApertarGatilho()
    {
        Raycast();
        PedirProjetil();
        Atirando();
        

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
            hitTransform = raycastHit.point;
        }
    }

    private void Atirando()
    {
        projetil.transform.position = pontaDaArma.position;
        projetil.gameObject.SetActive(true);
        projetil.DefineTargetPosition(hitTransform);
    }



}
