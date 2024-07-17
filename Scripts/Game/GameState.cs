using DG.Tweening;
using System;
using UnityEngine;

public class GameState : MonoBehaviour, IInitializable
{
    public static GameState Instance;

    public Action GameStarted;
    public Action GamePaused;
    public Action GameUnpaused;
    public Action GameFinished;

    public enum State
    {
        InGame,
        Paused,
        Finished
    }
    public State CurrentState { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }
    public void Initialize()
    {
        StartGame();
    }
    public void ChangeState(State state)
    {
        CurrentState = state;
    }
    public void StartGame()
    {
        GameStarted?.Invoke();
        CurrentState = State.InGame;
        Time.timeScale = 1.0f;
    }
    public void PauseGame()
    {
        GamePaused?.Invoke();
        CurrentState = State.Paused;
        Time.timeScale = 0.0f;
    }
    public void UnpauseGame()
    {
        GameUnpaused?.Invoke();
        CurrentState = State.InGame;
        Time.timeScale = 1.0f;
    }
    public void FinishGame()
    {
        GameFinished?.Invoke();
        CurrentState = State.Finished;
        GameOver.Instance.ShowGameOver();

        Camera.main.DOShakePosition(0.4f, 0.2f, fadeOut: true).SetUpdate(true);
        Camera.main.DOShakeRotation(0.4f, 0.2f, fadeOut: true).SetUpdate(true);
    }
}
