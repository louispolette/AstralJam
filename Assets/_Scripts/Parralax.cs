using Unity.VisualScripting;
using UnityEditor.Build.Content;
using UnityEngine;

public class Parralax : MonoBehaviour
{
    //private GameManager manager;
    public Camera cam;
    public Transform subjects;


    float startPos;
    float startZ;

    float Length;
    
    float travel => (float)cam.transform.position.y - startPos;

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

    // Update is called once per frame
    void Update()
    {
        float newDistance = cam.transform.position.y * parralaxFactor;
        float newMouvement = cam.transform.position.y * (1 - parralaxFactor);


        transform.position = new Vector3(0f, startPos + newDistance, transform.position.z);

        if(newMouvement > startPos + Length)
        {
            startPos += Length;
        }
        else if (newMouvement < startPos - Length)
        {
            startPos -= Length;
        }
    }
}
