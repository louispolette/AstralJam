using UnityEngine;

public class StarLine : MonoBehaviour
{
    [field: Space]

    [field: SerializeField] public float MinLength { get; private set; } = 1f;
    [field: SerializeField] public float MaxLength { get; private set; } = 5f;

    [field: Space]

    [field: SerializeField] public float StarRotationMult { get; private set; } = 1f;
    [field: SerializeField] public LayerMask CollisionCheckMask { get; private set; }

    [field: Space]

    [field: SerializeField] public float StarSpinVel { get; private set; } = 1000f;
    [field: SerializeField] public float StarSpinDeceleration { get; private set; } = 1f;

    [field: Space]

    [field: SerializeField] public GameObject StartStarObject { get; private set; }
    [field: SerializeField] public GameObject EndStarObject { get; private set; }

    private StarlineStar[] _stars = new StarlineStar[2];

    private Vector2 _drawStartPos;

    private float _lineDrawnTime = -9999f;
    private float _starVel = 0f;

    private int _linesDrawn = 0;

    private Camera _camera;
    private LineRenderer _line;

    private Vector3[] _linePosArray = new Vector3[2];

    private StarlineState _state = StarlineState.None;

    private const float LINE_LIFETIME = 2.5f;

    private Vector2 MousePos => _camera.ScreenToWorldPoint(Input.mousePosition);
    private Vector2 StarVec => _stars[1].transform.position - _stars[0].transform.position;
    private Vector2 StarDir => (_stars[1].transform.position - _stars[0].transform.position).normalized;
    private float StarlineLength => (_stars[1].transform.position - _stars[0].transform.position).magnitude;


    [System.Serializable]
    public class StarlineStar
    {
        public SpriteRenderer renderer;
        public Transform transform;
        public GameObject gameObject;

        public StarlineStar (GameObject obj)
        {
            renderer = obj.GetComponentInChildren<SpriteRenderer>();
            transform = obj.transform;
            gameObject = obj;
        }
    }

    private void Awake()
    {
        _stars[0] = new StarlineStar(StartStarObject);
        _stars[1] = new StarlineStar(EndStarObject);
        SetStarsVisible(false);

        _line = GetComponentInChildren<LineRenderer>();
        _line.positionCount = 2;
        _line.enabled = false;
        _camera = Camera.main;
    }

    private void Update()
    {
        HandleInput();

        if (_state == StarlineState.BeingDrawn)
        {
            UpdateStarRotation();
            UpdateStarPositions();
        }

        if (_state == StarlineState.Solid)
        {
            if (Time.time - _lineDrawnTime > LINE_LIFETIME)
            {
                ExpireLine();
            }
        }

        if (_state == StarlineState.Used)
        {
            DoStarSpin();
        }

        if ( _state == StarlineState.None)
        {

        }
        else
        {
            UpdateLine();
        }
    }

    private void FixedUpdate()
    {
        if (_state == StarlineState.Solid)
        {
            DoPlayerCheck();
        }
    }

    private void HandleInput()
    {
        if (GameManager.Instance.CurrentGameState == GameManager.GameState.GameOver)
        {
            if (Input.GetMouseButton(0) && _state == StarlineState.BeingDrawn)
            {
                ExpireLine();
            }

            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            BeginDraw();
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (StarlineLength >= MinLength)
            {
                CompleteLine();
            }
            else
            {
                ExpireLine();
            }
        }
    }

    private void UpdateStarPositions()
    {
        _stars[0].transform.position = _drawStartPos;

        Vector2 endPos = MousePos;
        Vector2 startToMouseVec = MousePos - (Vector2)_stars[0].transform.position;
        float targetDistance = startToMouseVec.magnitude;

        if (targetDistance > MaxLength)
        {
            float error = targetDistance - MaxLength;
            endPos = MousePos - startToMouseVec.normalized * error;
        }

        _stars[1].transform.position = endPos;
    }

    private void DoStarSpin()
    {
        float spinAmount = _starVel * Time.deltaTime;

        _stars[0].transform.eulerAngles = new Vector3(0f, 0f, _stars[0].transform.eulerAngles.z + spinAmount);
        _stars[1].transform.eulerAngles = new Vector3(0f, 0f, _stars[1].transform.eulerAngles.z - spinAmount);

        _starVel = Mathf.Max(0f, _starVel - StarSpinDeceleration * Time.deltaTime);
    }

    private void BeginDraw()
    {
        _state = StarlineState.BeingDrawn;
        _drawStartPos = MousePos;
        _line.enabled = true;
        SetStarsVisible(true);
        _lineDrawnTime = Time.time;
    }

    private void CompleteLine()
    {
        _state = StarlineState.Solid;

        if (_linesDrawn <= 0)
        {
            GameManager.UnpauseGame();
            GameManager.ShowGameUI();
        }

        _linesDrawn++;
    }

    private void ExpireLine()
    {
        _line.enabled = false;
        SetStarsVisible(false);
        _state = StarlineState.None;
    }

    private void OnBounce()
    {
        _state = StarlineState.Used;
        _starVel = StarSpinVel;
        _lineDrawnTime = Time.time;
    }

    private void SetStarsVisible(bool isVisible)
    {
        foreach (var star in _stars)
        {
            star.renderer.enabled = isVisible;
        }
    }

    private void UpdateStarRotation()
    {
        float starDistance = Vector2.Distance(_stars[0].transform.position, _stars[1].transform.position);
        _stars[0].transform.right = StarDir;
        _stars[0].transform.eulerAngles += new Vector3(0f, 0f, -starDistance * StarRotationMult);
        _stars[1].transform.right = StarDir;
        _stars[1].transform.eulerAngles += new Vector3(0f, 0f, starDistance * StarRotationMult);
    }

    private void UpdateLine()
    {
        _linePosArray[0] = Vector2.Lerp(_stars[0].transform.position, _stars[1].transform.position, 0.1f);
        _linePosArray[1] = Vector2.Lerp(_stars[0].transform.position, _stars[1].transform.position, 0.9f);
        _line.SetPositions(_linePosArray);
    }

    private void SetLineAlpha(float alpha)
    {

    }

    private void DoPlayerCheck()
    {
        var hit = Physics2D.Linecast(_stars[0].transform.position, _stars[1].transform.position, CollisionCheckMask);

        if (!hit) return;

        var player = hit.rigidbody.GetComponent<Player>();

        if (player != null)
        {
            player.Bounce(GetPerpDir());
            OnBounce();
        }
    }

    private Vector2 GetPerpDir()
    {
        var perp = Vector2.Perpendicular(_stars[1].transform.position - _stars[0].transform.position);

        if (perp.y < 0f)
        {
            perp = -perp;
        }

        return perp.normalized;
    }
}
