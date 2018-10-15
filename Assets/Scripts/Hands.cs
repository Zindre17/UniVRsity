using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hands : MonoBehaviour {


    private Rigidbody rb;

    private bool canFire = true;

    private bool grabbing = false;

    private Manager manager;

    private Focusable prevHover;

	void Start () {
        rb = gameObject.GetComponent<Rigidbody>();
        manager = Manager.instance;
	}
	
	void Update () {
        

        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);

        if(Physics.Raycast(ray, out hit)) {
            Focusable focusable = hit.collider.GetComponent<Focusable>();

            if (focusable != null) {
                if (focusable != prevHover) {
                    prevHover = focusable;
                    focusable.StartHover();
                    Debug.Log("start hover");
                }
                if (TriggerPressed() && canFire) {
                    canFire = false;
                    focusable.Toggle();
                }
            } else {
                if (prevHover != null) {
                    prevHover.EndHover();
                    Debug.Log("end hover");
                    prevHover = null;
                }
            }
        } else {
            if(prevHover != null) {
                prevHover.EndHover();
                prevHover = null;
            }
        }

        
        if (TriggerReleased() && !canFire) {
            canFire = true;
        }
    }

    private bool TriggerReleased() {
        return false;//device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger).x < 0.2f;
    }

    private bool TriggerPressed() {
        return false;//device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger).x > 0.8f;
    }


    private void OnCollisionEnter(Collision collision) {
        
    }

    private void OnCollisionStay(Collision collision) {
        
    }

    private void OnCollisionExit(Collision collision) {
        
    }
}
