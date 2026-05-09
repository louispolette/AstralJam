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

    [field: SerializeField] public GameObject StartStarObject { get; private set; }
    [field: SerializeField] public GameObject EndStarObject { get; private set; }

    private StarlineStar[] _stars = new StarlineStar[2];

    private Vector2 _drawStartPos;

    private float _lineDrawnTime = -9999f;

    private Camera _camera;
    private LineRenderer _line;

    private Vector3[] _linePosArray = new Vector3[2];

    private StartlineState _state = StartlineState.None;

    private const float LINE_LIFETIME = 4f;

    private Vector2 MousePos => _camera.ScreenToWorldPoint(Input.mousePosition);
    private Vector2 StarVec => (_stars[1].transform.position - _stars[0].transform.position).normalized;
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

        if (_state == StartlineState.BeingDrawn)
        {
            UpdateStarRotation();
            UpdateStarPositions();
        }

        if (_state == StartlineState.Solid)
        {
            if (Time.time - _lineDrawnTime > LINE_LIFETIME)
            {
                ExpireLine();
            }
        }

        if ( _state == StartlineState.None)
        {

        }
        else
        {
            UpdateLine();
        }
    }

    private void FixedUpdate()
    {
        if (_state == StartlineState.Solid)
        {
            DoPlayerCheck();
        }
    }

    private void HandleInput()
    {
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

    private void BeginDraw()
    {
        _state = StartlineState.BeingDrawn;
        _drawStartPos = MousePos;
        _line.enabled = true;
        SetStarsVisible(true);
        _lineDrawnTime = Time.time;
    }

    private void CompleteLine()
    {
        _state = StartlineState.Solid;
    }

    private void ExpireLine()
    {
        _line.enabled = false;
        SetStarsVisible(false);
        _state = StartlineState.None;
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
        _stars[0].transform.right = StarVec;
        _stars[0].transform.eulerAngles += new Vector3(0f, 0f, -starDistance * StarRotationMult);
        _stars[1].transform.right = StarVec;
        _stars[1].transform.eulerAngles += new Vector3(0f, 0f, starDistance * StarRotationMult);
    }

    private void UpdateLine()
    {
        _linePosArray[0] = _stars[0].transform.position;
        _linePosArray[1] = _stars[1].transform.position;
        _line.SetPositions(_linePosArray);
    }

    private void DoPlayerCheck()
    {
        var hit = Physics2D.Linecast(_stars[0].transform.position, _stars[1].transform.position, CollisionCheckMask);

        if (!hit) return;

        var player = hit.rigidbody.GetComponent<Player>();

        if (player != null)
        {
            player.Bounce(GetPerpDir());
            ExpireLine();
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
