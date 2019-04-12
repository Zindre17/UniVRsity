using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hoverable : MonoBehaviour {

    public Color hoverColor;
    public Color illegalColor;

    private Outline outline;
    private bool on = true;
    private bool legal = true;
    public bool Legal {
        get { return legal; }
        set {
            if(legal != value) {
                legal = value;
                UpdateColor();
            }
        }
    }

    private void Awake() {
        outline = gameObject.AddComponent<Outline>();
        outline.OutlineColor = hoverColor;
        outline.OutlineWidth = 0;
    }

    private void UpdateColor() {
        if (legal)
            outline.OutlineColor = hoverColor;
        else
            outline.OutlineColor = illegalColor;
    }
   
    public void StartHover(bool legal) {
        if (on) {
            outline.OutlineWidth = 5;
        }
    }

    public void StartHover() {
        if (on) {
            outline.OutlineWidth = 5;
        }
    }

    public void EndHover() {
        outline.OutlineWidth = 0;
    }

    public void Disable() {
        on = false;
    }

    public void Enable() {
        on = true;
    }
}

