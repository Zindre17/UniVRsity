using UnityEngine;

public class Focusable : MonoBehaviour {

    public Outline outline;
    public Color focusedColor;
    public Color hoverColor;
    public Color illegalColor;

    private bool inFocus = false;

    private Manager manager;
    private Renderer rend;
    private Color defCol; 

    void Start() {
        manager = Manager.instance;
        outline.OutlineColor = hoverColor;
        outline.OutlineWidth = 0;
        rend = GetComponent<Renderer>();
        defCol = rend.material.color;
    }

    public void Toggle() {
        if (inFocus) {
            DeFocus();
        } else {
            Focus();
        }
        Debug.Log(outline.OutlineWidth);
    }

    public bool IsFocused() {
        return inFocus;
    }

    private void Focus() {
        if (manager.CanFocus()) {
            inFocus = true;
            manager.AddInFocus(gameObject);
            outline.OutlineWidth = 0;
            rend.material.color = focusedColor;
        }
    }

    private void DeFocus() {
        if (inFocus) {
            manager.SubInFocus(gameObject);
            inFocus = false;
            outline.OutlineWidth = 0;
            rend.material.color = defCol;
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
