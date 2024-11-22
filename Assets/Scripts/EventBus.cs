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

    public event Action onInteragindo;

    public event Action <bool, bool>onTP;

    public event Action <float, Action> onFadeIn;
    public event Action <float, Action> onFadeOut;

    public event Action <string, int, bool> onColetaItem;

    public event Action<bool> onAtivaInimigos;

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

    public void Interagindo()
    {
        onInteragindo?.Invoke();
    }

    public void TP(bool aux1, bool aux2)
    {
        onTP?.Invoke(aux1, aux2);
    }

    public void FadeIn(float duracao, Action posFade)
    {
        onFadeIn?.Invoke(duracao, posFade);
    }
    public void FadeOut(float duracao, Action posFade)
    {
        onFadeOut?.Invoke(duracao, posFade);
    }
    
    public void ColetaItem(string nomeItem, int quantidadeItem, bool itemColetado)
    {
        onColetaItem?.Invoke(nomeItem, quantidadeItem, itemColetado);
    }

    public void AtivaInimigos(bool ativarInimigos)
    {
        onAtivaInimigos?.Invoke(ativarInimigos);
    }
}
