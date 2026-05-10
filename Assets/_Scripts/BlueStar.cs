using UnityEngine;

public class BlueStar : MonoBehaviour
{
    private float _usedTime = -9999f;

    private const float USE_COOLDOWN = 1.25f;

    private void FixedUpdate()
    {
        if (transform.position.y - GameManager.Instance.CamTarget.transform.position.y < -10f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        Debug.Log("Collision");
        if (Time.time - _usedTime < USE_COOLDOWN) return;

        var player = collision.GetComponentInParent<Player>();

        if (player != null)
        {
            Vector2 dir = (Random.Range(0, 2)) == 1 ? new Vector2(0.5f, 1f) : new Vector2(-0.5f, 1f);

            player.Bounce(dir, true);
            _usedTime = Time.time;
        }
    }
}
