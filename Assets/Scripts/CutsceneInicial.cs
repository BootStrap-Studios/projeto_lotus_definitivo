using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneInicial : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(IrProJogo());
        AudioManager.instance.PlayOneShot(FMODEvents.instance.cutsceneInicial, transform.position);
    }
    private IEnumerator IrProJogo()
    {
        yield return new WaitForSeconds(190f);

        SceneManager.LoadScene("Implemenetacao");
    }
}
