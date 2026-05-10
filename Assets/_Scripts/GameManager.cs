using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [field: Space]

    [field: SerializeField] public float MaxHeight { get; private set; } = 100f;

    [field: Header("References")]

    [field: SerializeField] public Player Player { get; private set; }
    [field: SerializeField] public CameraTarget CamTarget { get; private set; }
    [field: SerializeField] public TextMeshProUGUI ScoreText { get; private set; }
    [field: SerializeField] public Animator UIAnimator { get; private set; }
    [field: SerializeField] public GameObject GameOverText { get; private set; }

    private float _initialHeight;
    private int _height;

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

        Time.timeScale = 0f;
        _initialHeight = CamTarget.transform.position.y;
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
        Time.timeScale = 1f;
        Instance.CurrentGameState = GameState.Gameplay;
    }

    public static void ShowGameUI()
    {
        Instance.UIAnimator.SetTrigger("switch");
    }

    private void GameOver()
    {
        CurrentGameState = GameState.GameOver;
        GameOverText.SetActive(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
