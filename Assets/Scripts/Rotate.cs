using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public Vector3 rotationVector;

    // Update is called once per frame
    void Update()
    {
        transform.localEulerAngles += rotationVector * Time.deltaTime;
    }
}