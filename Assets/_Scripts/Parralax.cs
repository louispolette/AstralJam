using UnityEditor.Build.Content;
using UnityEngine;

public class Parralax : MonoBehaviour
{
    //private GameManager manager;
    public Camera cam;
    public Transform subjects;


    Vector2 startPos;
    float startZ;
    
    Vector2 travel => (Vector2)cam.transform.position - startPos;

    float distanceFromSubject => startZ - subjects.position.z;

    float clippingPlane => (cam.transform.position.z + (distanceFromSubject > 0 ? cam.farClipPlane : cam.nearClipPlane));

    float parralaxFactor => Mathf.Abs(distanceFromSubject / clippingPlane);
    private void Awake()
    {
        startPos = transform.position;
        startZ = transform.position.z;

        transform.position = new Vector3(0f, startPos.y, startZ / 100f);
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //subjects = GameManager.Instance.Player.CameraTarget;
        //cam = GameManager.Instance.Camera;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 newPos = startPos + travel * parralaxFactor;

        transform.position = new Vector3(0f, newPos.y,transform.position.z);
    }
}
