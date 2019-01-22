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

    public delegate void CompleteAction();
    public event CompleteAction OnComplete;

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
            if (OnComplete != null) OnComplete();
        }
    }

    private IEnumerator MoveTo(Vector3 point) {
        float dist = Vector3.Distance(point, transform.position);
        float distTraveled = 0f;
        Vector3 direction = (point - transform.position).normalized;
        while (distTraveled < dist) {
            distTraveled += speed * Time.deltaTime;
            transform.position = transform.position + direction * speed * Time.deltaTime;
            yield return null;
        }
        transform.position = point;
        MoveToNext();
    }
    
}
