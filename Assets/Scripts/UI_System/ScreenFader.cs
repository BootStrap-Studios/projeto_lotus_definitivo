using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    [SerializeField] private Image fader;

    private bool ocupado;

    private void OnEnable()
    {
        EventBus.Instance.onFadeIn += FadeIn;
        EventBus.Instance.onFadeOut += FadeOut;
    }

    private void OnDisable()
    {
        EventBus.Instance.onFadeIn -= FadeIn;
        EventBus.Instance.onFadeOut -= FadeOut;
    }

    public void FadeIn(float duracao, Action posFade)
    {
        if (ocupado) return;
        StartCoroutine(CO_FadeIn(duracao, posFade));
    }

    public void FadeOut(float duracao, Action posFade)
    {

        if (ocupado) return;
        StartCoroutine(CO_FadeOut(duracao, posFade));
    }

    private IEnumerator CO_FadeIn(float duracao, Action posFade)
    {
        ocupado = true;
        while (fader.color.a < 1)
        {
            fader.color = new Color(0, 0, 0, fader.color.a + (Time.deltaTime / duracao));
            yield return null;
        }
        fader.color = new Color(0, 0, 0, 1);
        ocupado = false;
        posFade?.Invoke();
        yield return null;
    }

    private IEnumerator CO_FadeOut(float duracao, Action posFade)
    {
        ocupado = true;
        while (fader.color.a > 0)
        {
            fader.color = new Color(0, 0, 0, fader.color.a - (Time.deltaTime / duracao));
            yield return null;
        }
        fader.color = new Color(0, 0, 0, 0);
        ocupado = false;
        posFade?.Invoke();
        yield return null;
    }
}
