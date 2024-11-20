using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class Botao : MonoBehaviour
{
    [SerializeField] private Button botao;
    [SerializeField] private TextMeshProUGUI botaoTXT;
    private Color corTXT;

    private void Awake()
    {
        corTXT = botaoTXT.color;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
