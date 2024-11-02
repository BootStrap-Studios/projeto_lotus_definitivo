using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemColetado : MonoBehaviour
{
    [SerializeField] private Image spriteItemAtual;
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
            spriteItemAtual.color = new Color(spriteItemAtual.color.r, spriteItemAtual.color.g, spriteItemAtual.color.b, spriteItemAtual.color.a - (Time.deltaTime / velFadeOut));
            quantidadeItemAtualTXT.color = new Color(1, 1, 1, quantidadeItemAtualTXT.color.a - (Time.deltaTime / velFadeOut));

            if (spriteItemAtual.color.a <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    public void ConfigurandoItem(Color sprite, string texto)
    {
        spriteItemAtual.color = sprite;
        quantidadeItemAtualTXT.text = texto;
    }
}
