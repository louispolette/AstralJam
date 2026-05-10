using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [field: Space]

    [field: SerializeField] public float StarSpawnthreshold { get; private set; } = 100f;
    [field: SerializeField] public float StarSpawnIncrement { get; private set; } = 50f;

    [field: Header("Time Scale")]

    [field: SerializeField] public float StartTimeScale { get; private set; } = 0.75f;
    [field: SerializeField] public float TimeScaleIncreaseMult { get; private set; } = 1f;

    [field: Header("References")]

    [field: SerializeField] public Player Player { get; private set; }
    [field: SerializeField] public CameraTarget CamTarget { get; private set; }
    [field: SerializeField] public TextMeshProUGUI ScoreText { get; private set; }
    [field: SerializeField] public Animator UIAnimator { get; private set; }
    [field: SerializeField] public GameObject GameOverText { get; private set; }
    [field: SerializeField] public GameObject TutorialStuff { get; private set; }
    [field: SerializeField] public BlueStarSpawner StarSpawner { get; private set; }

    private float _initialHeight;
    private int _height;
    private float _gameOverTime = float.PositiveInfinity;
    private float _nextStarHeight = 100f;
    public int jumps = 0;
    public GameState CurrentGameState { get; private set; } = GameState.Title;

    private static  bool hasSetResolution = false;

    public enum GameState
    {
        Title,
        Gameplay,
        GameOver
    }

    public static int highscore = 0;

    private void Awake()
    {
        Instance = this;

        Application.targetFrameRate = 120;
        Time.timeScale = 0f;
        _initialHeight = CamTarget.transform.position.y;
        _nextStarHeight = StarSpawnthreshold;

        if (!hasSetResolution)
        {
            Screen.SetResolution(1296, 976, Screen.fullScreen);
            hasSetResolution = true;
        }
    }

    private void Update()
    {
        if (CurrentGameState == GameState.Title || CurrentGameState == GameState.GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        if (CurrentGameState == GameState.GameOver && Input.GetMouseButtonDown(0))
        {
            if (Time.unscaledTime - _gameOverTime >= 1.5f)
            ReloadScene();
        }

        if (CurrentGameState == GameState.Gameplay)
        {
            UpdateTimescale();
        }
    }

    private void FixedUpdate()
    {
        CheckIfPlayerFell();
        UpdateScore();
        HandleStarSpawning();
    }

    private void CheckIfPlayerFell()
    {
        if (Player.transform.position.y - CamTarget.transform.position.y < -10f)
        {
            GameOver();
        }
    }

    private void HandleStarSpawning()
    {
        if (_height >= _nextStarHeight)
        {
            StarSpawner.SpawnStar();
            _nextStarHeight += StarSpawnIncrement * Random.Range(0.5f, 2f);
        }
    }

    private void UpdateScore()
    {
        _height = Mathf.FloorToInt((CamTarget.transform.position.y - _initialHeight) * 10f);
        ScoreText.text = _height.ToString();
    }

    public static void UnpauseGame()
    {
        Time.timeScale = Instance.StartTimeScale;
        Instance.TutorialStuff.SetActive(false);
        Instance.CurrentGameState = GameState.Gameplay;
    }

    public static void ShowGameUI()
    {
        Instance.UIAnimator.SetTrigger("switch");
    }

    private void GameOver()
    {
        if (CurrentGameState == GameState.GameOver) return;

        Player.gameObject.SetActive(false);

        CurrentGameState = GameState.GameOver;
        GameOverText.SetActive(true);
        _gameOverTime = Time.unscaledTime;
    }

    private void UpdateTimescale()
    {
        Time.timeScale = StartTimeScale + (_height * 0.0001f) * TimeScaleIncreaseMult;
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
