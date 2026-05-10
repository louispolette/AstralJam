using UnityEngine;

public class BlueStar : MonoBehaviour
{
    [Space]

    public float spinVel = 2000f;
    public float starSpinDeceleration = 4750f;

    private float _usedTime = -9999f;
    private float _starVel = 0f;

    private const float USE_COOLDOWN = 1.25f;

    private void FixedUpdate()
    {
        if (transform.position.y - GameManager.Instance.CamTarget.transform.position.y < -10f)
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        DoStarSpin();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Time.time - _usedTime < USE_COOLDOWN) return;

        var player = collision.GetComponentInParent<Player>();

        if (player != null)
        {
            Vector2 dir = (Random.Range(0, 2)) == 1 ? new Vector2(0.5f, 1f) : new Vector2(-0.5f, 1f);
            _starVel = (dir.x > 0f) ? spinVel : -spinVel;

            player.Bounce(dir, true);
            _usedTime = Time.time;
        }
    }

    private void DoStarSpin()
    {
        float spinAmount = _starVel * Time.deltaTime;

        transform.eulerAngles = new Vector3(0f, 0f, transform.eulerAngles.z + spinAmount);

        _starVel = Mathf.Max(0f, Mathf.Abs(_starVel) - starSpinDeceleration * Time.deltaTime) * Mathf.Sign(_starVel);
    }
}
