using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventBus : MonoBehaviour
{
    public static EventBus Instance { get; private set; }

    public event Action onPauseGame;

    public event Action onGameOver;

    public event Action <float, Action> onFadeIn;
    public event Action <float, Action> onFadeOut;

    public event Action <string, float> onColetaItem;

    private void Awake()
    {
        Instance = this;
    }

    public void PauseGame()
    {
        onPauseGame?.Invoke();
    }

    public void GameOver()
    {
        onGameOver?.Invoke();
    }  

    public void FadeIn(float duracao, Action posFade)
    {
        onFadeIn?.Invoke(duracao, posFade);
    }
    public void FadeOut(float duracao, Action posFade)
    {
        onFadeOut?.Invoke(duracao, posFade);
    }
    
    public void ColetaItem(string nomeItem, float quantidadeItem)
    {
        onColetaItem?.Invoke(nomeItem, quantidadeItem);
    }
}
