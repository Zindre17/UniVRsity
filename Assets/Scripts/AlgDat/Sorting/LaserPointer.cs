using UnityEngine;

public class LaserPointer : MonoBehaviour {

    private readonly float thickness = 0.01f;
    private readonly float maxLength = 20f;

    public GameObject beam;

    private void Start()
    {
        beam.transform.localScale = new Vector3(thickness, thickness, maxLength);
    }

    private void Update()
    {
        RaycastHit hit;
        Ray ray = new Ray(beam.transform.position, beam.transform.forward);
        float length = maxLength;
        if(Physics.Raycast(ray, out hit, maxLength))
        {
            length = hit.distance;
        }
        beam.transform.localScale = new Vector3( thickness, thickness,length);
    }
}
