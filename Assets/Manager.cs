using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour {

    public static Manager instance;

    public int maxInFocus = 2;
    private int inFocus = 0;

    void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public bool CanFocus() {
        return inFocus < maxInFocus;
    }

    public void AddInFocus() {
        if(CanFocus())
            inFocus++;
        Debug.Log(inFocus);
    }
    public void SubInFocus() {
        if(inFocus > 0)
            inFocus--;
        Debug.Log(inFocus);
    }

}
