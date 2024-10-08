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
}
