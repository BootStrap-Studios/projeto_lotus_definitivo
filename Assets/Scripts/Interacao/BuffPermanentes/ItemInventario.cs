using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemInventario : MonoBehaviour
{
    public Image imagem;
    [SerializeField] private TextMeshProUGUI texto;
    [SerializeField] private TextMeshProUGUI debito;


    public void AtualizaTexto(string nome, int quantidade)
    {
        texto.text = nome + ": " + quantidade.ToString();
        debito.enabled = false;
    }

    public void MostraDebito(int qntdDebito, bool ativaDebito)
    {
        debito.fontSize = texto.fontSize;
        debito.enabled = ativaDebito;
        debito.text = "-" + qntdDebito.ToString();
    }
}
