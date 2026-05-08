using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody2D RB { get; private set; }
    public Animator Animator { get; private set; }

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
    }
}
