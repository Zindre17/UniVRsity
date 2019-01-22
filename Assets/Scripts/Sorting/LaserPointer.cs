using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class LaserPointer : MonoBehaviour {

    public float thickness = 0.03f;
    public float maxLength = 20f;

    public GameObject pointer;

    private void Start()
    {
        if(pointer == null)
        {

        }

        transform.localScale = new Vector3(thickness, maxLength, thickness);
    }

    private void Update()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        float length = maxLength;
        if(Physics.Raycast(ray, out hit, maxLength))
        {
            length = hit.distance;
        }
        transform.localScale = new Vector3( thickness, thickness,length);
        //pointer.transform.localPosition = new Vector3(0, 0, length / 2);
    }
}
