using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemInventario : MonoBehaviour
{
    public Image imagem;
    [SerializeField] private TextMeshProUGUI texto;
    

    public void AtualizaTexto(string nome, int quantidade)
    {
        texto.text = nome + ": " + quantidade.ToString();
    }
}
