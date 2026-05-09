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

    private float _initialHeight;
    private int _height;

    private void Awake()
    {
        Instance = this;

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

    private void GameOver()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
