using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//[RequireComponent( typeof(Collider))]
public class Orb : MonoBehaviour
{
    [Serializable]
    public class OrbBreakEvent : UnityEvent { }
    public OrbBreakEvent breakEvent;

    private bool triggered = false;

    private void OnCollisionEnter(Collision collision) {
        if (collision.collider.CompareTag("Environment")){
            if (breakEvent != null && !triggered) {
                triggered = true;
                breakEvent.Invoke();
            }
        }
    }
}
