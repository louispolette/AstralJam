using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    private Player _player;

    private void Start()
    {
        _player = GameManager.Instance.Player;
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.jumps <= 0) return;

        if (transform.position.y < _player.transform.position.y)
        {
            transform.position = new Vector2(transform.position.x, _player.transform.position.y);
        }
    }
}
