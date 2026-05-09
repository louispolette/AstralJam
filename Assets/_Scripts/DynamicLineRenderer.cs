using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DynamicLineRenderer : MonoBehaviour
{    
    private Transform[] transformsToDraw;

    private LineRenderer _line;
    private LineRenderer Line
    {
        get
        {
            if (_line == null)
            {
                return GetComponent<LineRenderer>();
            }
            else
            {
                return _line;
            }
        }

        set => _line = value;
    }

    private void Awake()
    {
        Line = GetComponent<LineRenderer>();
    }

    private void OnDisable()
    {
        _line.enabled = false;
    }

    private void OnEnable()
    {
        _line.enabled = true;
    }

    public void Initialize(Transform[] targetTransforms)
    {
        transformsToDraw = targetTransforms;
        Line.enabled = true;
        Line.positionCount = targetTransforms.Length;
    }

    public void ClearTransforms()
    {
        transformsToDraw = null;
    }

    private void Update()
    {
        for (int i = 0; i < transformsToDraw.Length; i++)
        {
            _line.SetPosition(i, transformsToDraw[i].position);
        }
    }

    private void UpdateTest()
    {
        Line = GetComponent<LineRenderer>();

        Line.enabled = true;
        Line.positionCount = transformsToDraw.Length;

        Update();

        Line = null;
    }

    public void ClearLine()
    {
        Line = GetComponent<LineRenderer>();

        Line.positionCount = 0;

        Line = null;
    }
}
