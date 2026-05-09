using System.Collections.Generic;
using UnityEngine;

public class PlayerTailRenderer : MonoBehaviour
{
    [Header("TailSegments")]

    [field : SerializeField] public Transform[] TailBones { get; private set; }

    [Header("Rope")]

    [SerializeField] private float _ropeSegmentLength = 0.225f;

    [Header("Physics")]

    [SerializeField] private Vector2 _gravityForce = new Vector2(0f, -2f);
    [SerializeField, Range(0f, 1f)] private float _dampingFactor = 0.98f;
    [SerializeField] private float _correctionClampAmount = 0.1f;

    [Header("Constraints")]

    [SerializeField] private int _numOfConstraintRuns = 50;

    [Header("Gizmos")]

    [SerializeField] private bool _gizmosEnabled = false;

    private List<RopeSegment> _ropeSegments = new List<RopeSegment>();

    private Vector3 _ropeStartPoint;

    private void Start()
    {
        MakeRopeSegments();
    }

    private void FixedUpdate()
    {
        Simulate();

        for (int i = 0; i < _numOfConstraintRuns; i++)
        {
            ApplyConstraints();
        }

        UpdateTransforms();
    }

    private void MakeRopeSegments()
    {
        for (int i = 0; i < TailBones.Length; i++)
        {
            _ropeSegments.Add(new RopeSegment(TailBones[i]));
        }
    }

    private void Simulate()
    {
        for (int i = 0; i < _ropeSegments.Count; i++)
        {
            RopeSegment segment = _ropeSegments[i];
            Vector2 velocity = (segment.CurrentPosition - segment.OldPosition) * _dampingFactor;

            segment.OldPosition = segment.CurrentPosition;
            segment.CurrentPosition += velocity;
            segment.CurrentPosition += _gravityForce * Time.fixedDeltaTime;
            _ropeSegments[i] = segment;
        }
    }

    private void ApplyConstraints()
    {
        // Keep First point attached to self

        RopeSegment firstSegment = _ropeSegments[0];
        firstSegment.CurrentPosition = transform.position;
        _ropeSegments[0] = firstSegment;

        // Apply distance constraint

        for (int i = 0; i < TailBones.Length - 1; i++)
        {
            RopeSegment currentSeg = _ropeSegments[i];
            RopeSegment nextSeg = _ropeSegments[i + 1];

            float dist = (currentSeg.CurrentPosition - nextSeg.CurrentPosition).magnitude;
            float difference = (dist - _ropeSegmentLength);

            Vector2 changeDir = (currentSeg.CurrentPosition - nextSeg.CurrentPosition).normalized;
            Vector2 changeVector = changeDir * difference;

            if (i != 0)
            {
                currentSeg.CurrentPosition -= (changeVector * 0.5f);
                nextSeg.CurrentPosition += (changeVector * 0.5f);
            }
            else
            {
                nextSeg.CurrentPosition += changeVector;
            }

            _ropeSegments[i] = currentSeg;
            _ropeSegments[i + 1] = nextSeg;
        }
    }

    private void UpdateTransforms()
    {
        for (int i = 0; i < _ropeSegments.Count; i++)
        {
            _ropeSegments[i].UpdateTransformPosition();
            if (i < _ropeSegments.Count - 1)
            {
                _ropeSegments[i].transform.right = _ropeSegments[i].transform.position - _ropeSegments[i + 1].transform.position;
            }
            else
            {
                _ropeSegments[i].transform.right = _ropeSegments[i - 1].transform.right;
            }
        }
    }

    public struct RopeSegment
    {
        public Transform transform;

        public Vector2 CurrentPosition;
        public Vector2 OldPosition;

        public RopeSegment(Transform transform)
        {
            this.transform = transform;
            CurrentPosition = transform.position;
            OldPosition = transform.position;
        }

        public void UpdateTransformPosition()
        {
            transform.position = CurrentPosition;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!_gizmosEnabled) return;

        Gizmos.color = Color.yellow;
        
        foreach (var segment in _ropeSegments)
        {
            Gizmos.DrawSphere(segment.CurrentPosition, 0.1f);
        }
    }
#endif
}
