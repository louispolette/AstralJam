using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [field: Space]

    [field: SerializeField] public float MaxHeight { get; private set; } = 100f;

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

    private float _initialHeight;
    private int _height;
    private float _gameOverTime = float.PositiveInfinity;
    public int jumps = 0;
    public GameState CurrentGameState { get; private set; } = GameState.Title;

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
    }

    private void Update()
    {
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
    }

    private void CheckIfPlayerFell()
    {
        if (Player.transform.position.y - CamTarget.transform.position.y < -10f)
        {
            GameOver();
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
