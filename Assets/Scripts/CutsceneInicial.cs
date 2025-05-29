using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CutsceneInicial : MonoBehaviour
{
    [SerializeField] private GameObject botao;
    [SerializeField] private Slider botaoPular;
    float tempoSegurando = 0;

    private void Start()
    {
        StartCoroutine(IrProJogo());
        AudioManager.instance.PlayOneShot(FMODEvents.instance.cutsceneInicial, transform.position);
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            botao.SetActive(true);
        }
        if (Input.GetKey(KeyCode.Space))
        {
            tempoSegurando += Time.deltaTime / 2;
            botaoPular.value = Mathf.MoveTowards(botaoPular.value, tempoSegurando, 1 * Time.deltaTime);
        }
        else
        {
            tempoSegurando = 0;
            botaoPular.value = 0;
        }

        if(tempoSegurando >= 2)
        {
            SceneManager.LoadScene("Implemenetacao");
        }
    }
    private IEnumerator IrProJogo()
    {
        yield return new WaitForSeconds(190f);

        SceneManager.LoadScene("Implemenetacao");
    }
}
