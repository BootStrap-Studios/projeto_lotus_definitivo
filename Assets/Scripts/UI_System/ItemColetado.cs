using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemColetado : MonoBehaviour
{
    [SerializeField] private Image imagemItemAtual;
    [SerializeField] private Image background;
    [SerializeField] private TextMeshProUGUI quantidadeItemAtualTXT;
    [SerializeField] private float tempoAtivo;
    [SerializeField] private float velFadeOut;
    [SerializeField] private float velMexer;

    private void Update()
    {
        gameObject.transform.position = new Vector3(transform.position.x, transform.position.y + (Time.deltaTime / velMexer), transform.position.z);

        tempoAtivo -= Time.deltaTime;

        if (tempoAtivo <= 0)
        {
            imagemItemAtual.color = new Color(imagemItemAtual.color.r, imagemItemAtual.color.g, imagemItemAtual.color.b, imagemItemAtual.color.a - (Time.deltaTime / velFadeOut));
            background.color = new Color(background.color.r, background.color.g, background.color.b, background.color.a - (Time.deltaTime / velFadeOut));
            quantidadeItemAtualTXT.color = new Color(1, 1, 1, quantidadeItemAtualTXT.color.a - (Time.deltaTime / velFadeOut));

            if (imagemItemAtual.color.a <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    public void ConfigurandoItem(Sprite sprite, string texto)
    {
        imagemItemAtual.sprite = sprite;
        quantidadeItemAtualTXT.text = texto;
    }
}
