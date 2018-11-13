using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hoverable : MonoBehaviour {

    public Color hoverColor;
    public Color illegalColor;

    private Outline outline;
    private bool on = true;

    private void Awake() {
        outline = gameObject.AddComponent<Outline>();
        outline.OutlineColor = hoverColor;
        outline.OutlineWidth = 0;
    }
   
    public void StartHover(bool legal) {
        if (on) {
            if (legal)
                outline.OutlineColor = hoverColor;
            else
                outline.OutlineColor = illegalColor;
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

