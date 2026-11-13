using System;
using Unity.VisualScripting;
using UnityEngine;

public enum GameState
{
    Start,
    Playing,
    Paused,
    Victory,
    GameOver,
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("State Settings")]
    [SerializeField] private GameState currentState;

    [Header("Victory Settings (Enemigos)")]
    [SerializeField] private EnemyAI[] enemies;
    [SerializeField] private int totalEnemies;
    [SerializeField] private int enemiesRemaining;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (enemies == null || enemies.Length == 0)
        {
            enemies = FindObjectsByType<EnemyAI>(FindObjectsSortMode.None);
        }
        totalEnemies = enemies != null ? enemies.Length : 0;
        enemiesRemaining = totalEnemies;

        ChangeState(GameState.Playing);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Paused();
        }
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;
        switch (currentState)
        {
            case GameState.Start:
                Time.timeScale = 1f;
                Debug.Log("Inicio el juego");
                break;
            case GameState.Playing:
                Time.timeScale = 1f;
                Debug.Log("<color=green>ESTADO: jugando</color>");
                break;
            case GameState.Paused:
                Time.timeScale = 0f;
                Debug.Log("<color=yellow>ESTADO: pausa</color>");
                break;
            case GameState.Victory:
                Time.timeScale = 0f;
                Debug.Log("<color=cyan>ESTADO: Victoria</color>");
                break;
            case GameState.GameOver:
                Time.timeScale = 0f;
                Debug.Log("<color=red>ESTADO: Game Over</color>");
                break;
        }
    }

    public void Paused()
    {
        if (currentState == GameState.Playing)
        {
            ChangeState(GameState.Paused);
        }
        else if (currentState == GameState.Paused)
        {
            ChangeState(GameState.Playing);
        }
    }

    
    public void NotifyPlayerDead()
    {
        if (currentState != GameState.Playing) return;
        ChangeState(GameState.GameOver);
    }

    
    public void NotifyEnemyDefeat()
    {
        if (currentState != GameState.Playing) return;

        enemiesRemaining--;

        if (enemiesRemaining <= 0)
        {
            ChangeState(GameState.Victory);
        }
    }

    
    public void NotifyGoalZone()
    {
        if (currentState != GameState.Playing) return;

        ChangeState(GameState.Victory);
    }
}