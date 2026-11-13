using System;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    private static Texture2D whiteTexture;

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
    }

    void Start()
    {
        Time.timeScale = 1f;

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

        if ((currentState == GameState.GameOver || currentState == GameState.Victory) && Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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

    private void OnGUI()
    {
        if (currentState == GameState.Victory)
        {
            DrawBanner("¡VICTORIA!", "¡Colocaste la moneda en la plataforma!", new Color(0.1f, 0.85f, 0.95f));
        }
        else if (currentState == GameState.GameOver)
        {
            DrawBanner("¡GAME OVER!", "El enemigo te ha eliminado.", new Color(1f, 0.25f, 0.25f));
        }
    }

    private void DrawBanner(string title, string subtitle, Color titleColor)
    {
        if (whiteTexture == null)
        {
            whiteTexture = new Texture2D(1, 1);
            whiteTexture.SetPixel(0, 0, Color.white);
            whiteTexture.Apply();
        }

        // Fondo semitransparente oscuro en toda la pantalla
        Color prev = GUI.color;
        GUI.color = new Color(0f, 0f, 0f, 0.65f);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), whiteTexture);

        // Caja central
        float boxWidth = 500f;
        float boxHeight = 220f;
        float boxX = (Screen.width - boxWidth) / 2f;
        float boxY = (Screen.height - boxHeight) / 2f;

        GUI.color = new Color(0.1f, 0.12f, 0.15f, 0.95f);
        GUI.DrawTexture(new Rect(boxX, boxY, boxWidth, boxHeight), whiteTexture);
        GUI.color = prev;

        // Borde
        GUIStyle borderStyle = new GUIStyle();
        GUI.color = titleColor;
        GUI.DrawTexture(new Rect(boxX, boxY, boxWidth, 4), whiteTexture);
        GUI.DrawTexture(new Rect(boxX, boxY + boxHeight - 4, boxWidth, 4), whiteTexture);
        GUI.color = prev;

        // Título
        GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.fontSize = 38;
        titleStyle.alignment = TextAnchor.MiddleCenter;
        titleStyle.normal.textColor = titleColor;
        GUI.Label(new Rect(boxX, boxY + 25, boxWidth, 50), title, titleStyle);

        // Subtítulo
        GUIStyle subStyle = new GUIStyle(GUI.skin.label);
        subStyle.fontSize = 18;
        subStyle.alignment = TextAnchor.MiddleCenter;
        subStyle.normal.textColor = Color.white;
        GUI.Label(new Rect(boxX, boxY + 90, boxWidth, 35), subtitle, subStyle);

        // Mensaje de reinicio
        GUIStyle restartStyle = new GUIStyle(GUI.skin.label);
        restartStyle.fontStyle = FontStyle.Italic;
        restartStyle.fontSize = 16;
        restartStyle.alignment = TextAnchor.MiddleCenter;
        restartStyle.normal.textColor = new Color(0.85f, 0.85f, 0.85f);
        GUI.Label(new Rect(boxX, boxY + 145, boxWidth, 30), "Presiona [R] para reiniciar el nivel", restartStyle);
    }
}