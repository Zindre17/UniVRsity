using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Movable : MonoBehaviour {

    public int maxPoints = 10;
    public float speed = 1;
    public float radiusBuffer = 0.01f;

    private bool moving = false;
    public bool Moving {
        get { return moving; }
        set {
            if(moving != value) {
                moving = value;
                if (value) {
                    EventManager.StartedMovement();
                    MoveToNext();
                } else {
                    EventManager.FinishedMovement();
                }
            }
        }
    }
    private Vector3 direction;

    private Queue<Vector3> moveQueue;

    private void Start() {
        moveQueue = new Queue<Vector3>();
    }
	
    public void AddWayPoint(Vector3 point) {
        moveQueue.Enqueue(point);
        if (!Moving)
            Moving = true;
    }

    private void MoveToNext() {
        if(moveQueue.Count != 0) {
            StartCoroutine(MoveTo(moveQueue.Dequeue()));
        } else {
            Moving = false;
        }
    }

    private IEnumerator MoveTo(Vector3 point) {
        float dist = Vector3.Distance(point, transform.position);
        while(dist > radiusBuffer) {
            direction = (point - transform.position).normalized;
            dist = Vector3.Distance(point, transform.position);
            transform.position = transform.position + direction * speed * Time.deltaTime;
            yield return null;
        }
        MoveToNext();
    }
    
}
