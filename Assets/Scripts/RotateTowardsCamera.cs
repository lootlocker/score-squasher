using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTowardsCamera : MonoBehaviour
{
    public Camera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Screenshake.instance.transform.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        var lookPos = mainCamera.transform.position - transform.position;
        lookPos.z = 0f;
        lookPos.x = 0f;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = rotation;
    }
}
