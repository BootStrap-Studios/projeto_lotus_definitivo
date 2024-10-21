using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projetil : MonoBehaviour
{
    private Vector3 targetPosition;

    float moveSpeed = 15f;

    void Update()
    {
        MovimentarProjetil();
    }

    public void DefineTargetPosition(Vector3 target)
    {
        targetPosition = target;
    }

    private void MovimentarProjetil()
    {
        //float distanceBefore = Vector3.Distance(transform.position, targetPosition);

        Vector3 moveDir = (targetPosition - transform.position);
        transform.position += moveDir * moveSpeed * Time.deltaTime;

        //float distanceAfter = Vector3.Distance(transform.position, targetPosition);

        //if(distanceBefore < distanceAfter)
        //{
            //gameObject.SetActive(false);
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.name);

        if(other.CompareTag("Inimigo"))
        {
            //Debug.Log("Acertei o inimigo");
            other.GetComponent<Inimigo>().TomarDano(1f);
            gameObject.SetActive(false);
        } else
        {
            //Debug.Log("Acertei outra coisa");
            gameObject.SetActive(false);
        }
    }
}
