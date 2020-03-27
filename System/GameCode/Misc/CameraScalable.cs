using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScalable : MonoBehaviour
{
    public float ScaleOffset = 1.0f;
    private Vector3 initialScale;
    
    void Start()
    {
        initialScale = transform.localScale;
    }

    void Update()
    {
        Plane plane = new Plane(Camera.main.transform.forward, Camera.main.transform.position);
        float distantion = plane.GetDistanceToPoint(transform.position);
        transform.localScale = initialScale * distantion * ScaleOffset;
    }
}
