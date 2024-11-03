using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerSalas : MonoBehaviour
{
    [SerializeField] private Sala[] salas1;
    [SerializeField] private Sala[] salas2;
    [SerializeField] private Sala[] salas3;
    [SerializeField] private Sala[] salas4;
    [SerializeField] private Sala[] salas5;
    [SerializeField] private Sala[] salas6;
    [SerializeField] private Sala[] salas7;
    [SerializeField] private Sala[] salas8;
    [SerializeField] private Sala[] salas9;

    private Sala[] salasSorteadas;

    private int salaAtual;

    private PlayerMovement player;

    private void Start()
    {
        SorteandoSalas();
    }

    public void IrPraProxSala()
    {
        salasSorteadas[salaAtual + 1].gameObject.SetActive(true);

        player.transform.position = salasSorteadas[salaAtual + 1].posicaoInicial.position;

        salasSorteadas[salaAtual].gameObject.SetActive(false);

        salaAtual++;
    }


    private void SorteandoSalas()
    {
        salaAtual = -1;

        int i1 = Random.Range(0, salas1.Length);
        salasSorteadas[0] = salas1[i1];

        int i2 = Random.Range(0, salas2.Length);
        salasSorteadas[0] = salas1[i2];

        int i3 = Random.Range(0, salas3.Length);
        salasSorteadas[0] = salas1[i3];

        int i4 = Random.Range(0, salas4.Length);
        salasSorteadas[0] = salas1[i4];

        int i5 = Random.Range(0, salas5.Length);
        salasSorteadas[0] = salas1[i5];

        int i6 = Random.Range(0, salas6.Length);
        salasSorteadas[0] = salas1[i6];

        int i7 = Random.Range(0, salas7.Length);
        salasSorteadas[0] = salas1[i7];

        int i8 = Random.Range(0, salas8.Length);
        salasSorteadas[0] = salas1[i8];

        int i9 = Random.Range(0, salas9.Length);
        salasSorteadas[0] = salas1[i9];
    }


}
