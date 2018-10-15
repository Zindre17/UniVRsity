using UnityEngine;

public class CourseRenderer : MonoBehaviour {

    public Course course;
    public Transform glassContainer;
    
    // Use this for initialization
    void Start () {

        Instantiate(course.courseRepModel, glassContainer);

	}
}
