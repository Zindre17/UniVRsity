using UnityEngine;

public class LaserPointer : MonoBehaviour {

    private readonly float thickness = 0.01f;
    private readonly float maxLength = 20f;

    public GameObject pointer;

    private void Start()
    {
        transform.localScale = new Vector3(thickness, thickness, maxLength);
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
