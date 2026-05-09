using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [field: Space]

    [field: SerializeField] public float MaxHeight { get; private set; } = 100f;

    [field: Header("References")]

    [field: SerializeField] public Player Player { get; private set; }
    [field: SerializeField] public CameraTarget CamTarget { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
}
