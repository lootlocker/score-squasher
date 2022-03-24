using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screenshake : MonoBehaviour
{
    public static Screenshake instance;
    float shakeAmount;
    float shakeTimer = 0f;

    void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(shakeTimer > 0)
        {
            transform.localEulerAngles = new Vector3(0, 0, Random.Range(-shakeAmount, shakeAmount)) * shakeTimer;
            shakeTimer -= Time.deltaTime;
        }
        else
        {
            shakeTimer = 0f;
            transform.localEulerAngles = Vector3.zero;
        }
    }

    public void Shake(float amount, float timer)
    {
        shakeTimer = timer;
        shakeAmount = amount;
    }
}
