using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arma : MonoBehaviour
{
    [SerializeField] private LayerMask aimColliderMask;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            Shoot();
        }
    }


    private void Shoot()
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        Transform hitTransform = null;

        if(Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderMask))
        {
            hitTransform = raycastHit.transform;
        }

        if(hitTransform != null)
        {
            if(hitTransform.CompareTag("Inimigo"))
            {
                Debug.Log("Acertou o inimigo");
            } else
            {
                Debug.Log("Acertou outra coisa");
            }
        }
    }



}
