using System.Collections;
using System.Collections.Generic;
using UnityEngine;


interface IInteractable
{
    public void Interagir();
}
public class InteracaoPlayer : MonoBehaviour
{
    [SerializeField] private float rangeInteracao;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            Ray r = new Ray(transform.position, transform.forward);
            if(Physics.Raycast(r, out RaycastHit hitInfo, rangeInteracao))
            {
                if(hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
                {
                    interactObj.Interagir();
                }
            }
        }
    }
}
