using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public GameState CurrentState { get; private set; } = GameState.FreeExploration;
    public event Action<GameState> OnStateChanged;

    public static GameManager Instance { 
        get { 
            if (_instance == null)
            {
                Debug.LogError("GameManager instance is null. Make sure there is a GameManager in the scene.");
            }
            return _instance;
        }
        private set { 
            _instance = value; 
        } 
    }

    private void Awake()
    {
        if (_instance != this && _instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetState(GameState newState)
    {
        CurrentState = newState;
        OnStateChanged?.Invoke(newState);
    }

}