using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToPosition : MonoBehaviour
{

    public Transform transformToFollow;

    public float lerpSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, transformToFollow.position, Time.deltaTime*lerpSpeed);
    }
}
