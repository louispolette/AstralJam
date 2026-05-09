using UnityEngine;

public class Player : MonoBehaviour
{
    [field: Space]

    [field: SerializeField] public AnimationCurve DecelerationCurve;
    [field: SerializeField] public float DecelerationDuration = 1.5f;

    [field: Space]

    [field: SerializeField] public float FallAcceleration = 0.5f;
    [field: SerializeField] public float FallMaxVel = 10f;

    [field: Space]

    [field: SerializeField] public float BounceForce = 10f;

    private float _lastJumpTime = -9999f;
    private Vector2 _lastJumpInitialVel;

    private PlayerState _state = PlayerState.Falling;

    public enum PlayerState
    {
        Jumping,
        Falling
    }

    public Rigidbody2D RB { get; private set; }
    public Animator Animator { get; private set; }

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            transform.position = Vector2.zero;
            RB.linearVelocity = Vector2.zero;
        }
    }

    private void FixedUpdate()
    {
        UpdateGravity();
    }

    private void UpdateGravity()
    {
        _state = (RB.linearVelocityY <= 0) ? PlayerState.Falling : PlayerState.Jumping;

        if (_state == PlayerState.Falling)
        {
            RB.linearVelocityY -= FallAcceleration;
        }
        else if (_state == PlayerState.Jumping)
        {
            float jumpCompletion = (Time.time - _lastJumpTime) / DecelerationDuration;
            RB.linearVelocityY = Mathf.Lerp(_lastJumpInitialVel.y, 0, jumpCompletion);
            RB.linearVelocityX = Mathf.Lerp(Mathf.Abs(_lastJumpInitialVel.x), 0, jumpCompletion) * Mathf.Sign(RB.linearVelocityX);
        }
    }

    public void Bounce(Vector2 direction)
    {
        Vector2 force = direction * BounceForce;
        RB.linearVelocity = force;
        _lastJumpTime = Time.time;
        _lastJumpInitialVel = RB.linearVelocity;
        _state = PlayerState.Jumping;
    }

    public void Flip()
    {
        RB.linearVelocityX = -RB.linearVelocity.x;
    }
}
