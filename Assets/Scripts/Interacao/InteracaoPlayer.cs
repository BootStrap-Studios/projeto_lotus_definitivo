using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


interface IInteractable
{
    public void Interagir();
}
public class InteracaoPlayer : MonoBehaviour
{
    [SerializeField] private float rangeInteracao;
    [SerializeField] private Image botaoInteragir;

    // Update is called once per frame
    void Update()
    {
        Ray r = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(r, out RaycastHit hitInfo, rangeInteracao))
        {
            if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
            {
                botaoInteragir.gameObject.SetActive(true);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactObj.Interagir();                    
                }
            }
            else
            {
                botaoInteragir.gameObject.SetActive(false);
            }
        }
    }
}
