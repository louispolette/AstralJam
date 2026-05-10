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

    [field: Header("References")]

    [field: SerializeField] public SpriteRenderer BodySprite { get; private set; }
    [field: SerializeField] public SpriteRenderer HandSprite { get; private set; }
    [field: SerializeField] public AudioSource Audio { get; private set; }

    [field: Header("Audio")]

    [field: SerializeField] public AudioClip JumpSFX { get; private set; }

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
    public PlayerTailRenderer ScarfRenderer { get; private set; }
    public DynamicLineRenderer DynamicLine { get; private set; }

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        ScarfRenderer = GetComponentInChildren<PlayerTailRenderer>();
        DynamicLine = GetComponentInChildren<DynamicLineRenderer>();

        var scarfBones = ScarfRenderer.TailBones;
        DynamicLine.Initialize(scarfBones);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            transform.position = Vector2.zero;
            RB.linearVelocity = Vector2.zero;
        }

        UpdateGravity();
    }

    private void UpdateGravity()
    {
        var newState = (RB.linearVelocityY <= 0) ? PlayerState.Falling : PlayerState.Jumping;
        SetPlayerState(newState);

        if (_state == PlayerState.Falling)
        {
            RB.linearVelocityY = Mathf.Max(-FallMaxVel, RB.linearVelocityY - FallAcceleration * Time.deltaTime);
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
        FlipSprite();
        PlayJumpAudio();
        GameManager.Instance.jumps++;
        SetPlayerState(PlayerState.Jumping);
    }

    private void PlayJumpAudio()
    {
        float pitch = Random.Range(0.975f - 0.05f, 1.025f - 0.05f);
        Audio.pitch = pitch;
        Audio.PlayOneShot(JumpSFX, 0.075f);
    }

    public void FlipVel()
    {
        RB.linearVelocityX = -RB.linearVelocity.x;
        FlipSprite();
    }

    public void FlipSprite()
    {
        BodySprite.flipX = RB.linearVelocityX < 0f;
        HandSprite.flipX = RB.linearVelocityX < 0f;
    }

    private void SetPlayerState(PlayerState newState)
    {
        if (newState == _state) return;

        _state = newState;
        Animator.SetBool("isJumping", _state == PlayerState.Jumping);
    }
}
