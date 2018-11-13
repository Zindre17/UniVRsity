﻿using UnityEngine;

public class Movable : MonoBehaviour {

    public int maxPoints = 10;
    public float speed = 1;
    public float radiusBuffer = 0.01f;

    private int numberOfWayPoints = 0;
    private Vector3[] wayPoints;
    private int currentPoint = -1;
    private bool moving = false;
    private Vector3 direction;

    private void Start() {
        wayPoints = new Vector3[maxPoints];
    }
	
    public void SetPath(Vector3[] path) {
        numberOfWayPoints = path.Length;
        wayPoints = path;
        currentPoint = -1;
        EventManager.StartedMovement();
        Move();
    }

    public void SetPath(Vector3 point) {
        wayPoints = new Vector3[] { point };
        numberOfWayPoints = 1;
        currentPoint = -1;
        EventManager.StartedMovement();
        Move();
    }

	public void Move() {
        Debug.Log("current point : " + currentPoint);
        currentPoint++;
        if(currentPoint < numberOfWayPoints) {
            moving = true;
        } else {
            EventManager.FinishedMovement();
            moving = false;
            currentPoint = -1;
            numberOfWayPoints = 0;
            //Selectable s = GetComponentInChildren<Selectable>();
            //if(s!=null) s.Toggle();
        }
    }
	void Update () {
        if (moving ) {
            direction = (wayPoints[currentPoint] - transform.position).normalized;  
            transform.position += direction * speed * Time.deltaTime;
            if(Vector3.Distance(transform.position, wayPoints[currentPoint])< radiusBuffer) {
                Move();
            }
        }
	}
}
