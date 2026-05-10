using UnityEngine;

public class Parralax : MonoBehaviour
{
    //private GameManager manager;
    private GameManager manager;
    public Camera cam;
    public Transform subjects;

    public GameObject ObjectSprite;
    public GameObject ObjectSprite2;



    public SpriteRenderer _spriteRenderer;
    public SpriteRenderer _spriteRenderer2;


    public Sprite[] RandomSprite;
    public Sprite[] RandomSprite2;


    int SpriteChose = 0;

    public bool Particle = false;


    float startPos;
    float startZ;

    float RandomX = 0;
    public bool isRandomX = false;

    float Length;

    float distanceFromSubject => startZ - subjects.position.z;

    float clippingPlane => (cam.transform.position.z + (distanceFromSubject > 0 ? cam.farClipPlane : cam.nearClipPlane));

    public float parralaxFactor => Mathf.Abs(distanceFromSubject / clippingPlane);
    private void Awake()
    {
        startPos = transform.position.y;
        startZ = transform.position.z;

        Length = GetComponent<SpriteRenderer>().bounds.size.y;

        transform.position = new Vector3(0f, startPos, startZ / 100f);

        



    }

    private void Start()
    {
        subjects = GameManager.Instance.Player.transform;
        //cam = GameManager.Instance.CamTarget.GetComponent<Camera>();

        if (GetComponent<SpriteRenderer>() != null)
        {
            if (ObjectSprite.GetComponent<SpriteRenderer>() != null) _spriteRenderer = ObjectSprite.GetComponent<SpriteRenderer>();
            if (ObjectSprite2.GetComponent<SpriteRenderer>() != null) _spriteRenderer2 = ObjectSprite2.GetComponent<SpriteRenderer>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        float newDistance = cam.transform.position.y * parralaxFactor;
        float newMouvement = cam.transform.position.y * (1 - parralaxFactor);


        transform.position = new Vector3(RandomX, startPos + newDistance, transform.position.z);

        if(newMouvement > startPos + Length)
        {
            if(!Particle)
            {
                startPos += Length;
            }
            else
            {
                startPos += Length * 2;
            }

            if (RandomSprite.Length > 0)
            {
                SpriteChose = Random.Range(0, RandomSprite.Length);
                _spriteRenderer.sprite = RandomSprite2[SpriteChose];
                _spriteRenderer2.sprite = RandomSprite[SpriteChose];
            }

            if (isRandomX) RandomX = Random.Range(-3, 3);
        }
        else if (newMouvement < startPos - Length)
        {
            if (!Particle)
            {
                startPos -= Length;
            }
            else
            {
                startPos -= Length * 2;
            }

            if (isRandomX) RandomX = Random.Range(-3, 3);
        }
    }
}
