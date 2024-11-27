using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneInicial : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(IrProJogo());
    }
    private IEnumerator IrProJogo()
    {
        yield return new WaitForSeconds(185f);

        SceneManager.LoadScene("Implemenetacao");
    }
}
