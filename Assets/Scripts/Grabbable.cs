using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbable : MonoBehaviour {

    private bool isGrabbed = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PickedUp(Transform hand) {
        if (!isGrabbed) {
            isGrabbed = true;
            gameObject.transform.parent = hand;
        }
    }

    public void Dropped() {
        if (isGrabbed) {
            isGrabbed = false;
            gameObject.transform.parent = null;
        }
    }
}
