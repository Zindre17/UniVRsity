using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Focusable : MonoBehaviour {

    public Outline outline;
    public Color focusedColor;
    public Color hoverColor;
    public Color illegalColor;

    private bool inFocus = false;

    private Manager manager;

    void Start() {
        manager = Manager.instance;
        outline.OutlineColor = hoverColor;
        outline.OutlineWidth = 0;
    }

    public void Toggle() {
        if (inFocus) {
            DeFocus();
        } else {
            Focus();
        }
        Debug.Log(outline.OutlineWidth);
    }

    private void Focus() {
        if (manager.CanFocus()) {
            inFocus = true;
            manager.AddInFocus();
            outline.OutlineWidth = 10;
            transform.Translate(-Vector3.forward *2* transform.localScale.z);
        }
    }

    private void DeFocus() {
        if (inFocus) {
            manager.SubInFocus();
            inFocus = false;
            outline.OutlineWidth = 0;
            transform.Translate(Vector3.forward*2* transform.localScale.z);
        }
    }

    public void StartHover() {
        if (manager.CanFocus() || inFocus)
            outline.OutlineColor = hoverColor;
        else
            outline.OutlineColor = illegalColor;
        outline.OutlineWidth = 3;
    }

    public void EndHover() {
        outline.OutlineColor = focusedColor;
        if (inFocus) {
            outline.OutlineWidth = 10;
        } else {
            outline.OutlineWidth = 0;
        }
    }
}
