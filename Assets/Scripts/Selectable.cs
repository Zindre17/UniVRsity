using UnityEngine;

public class Selectable : MonoBehaviour {

    public Outline outline;
    public Color focusedColor;
    public Color hoverColor;
    public Color illegalColor;

    private bool selected = false;
    private bool correct = false;

    private SortManager manager;
    private Renderer rend;
    private Color defCol;
    public Color correctColor;

    void Start() {
        manager = SortManager.instance;
        outline.OutlineColor = hoverColor;
        outline.OutlineWidth = 0;
        rend = GetComponent<Renderer>();
        defCol = rend.material.color;
    }

    public void SetCorrect(bool b) {
        if (correct != b) {
            correct = b;
            if(rend != null) {

                if (correct) {
                    rend.material.color = correctColor;
                } else {
                    rend.material.color = defCol;
                }
            }
        }
    }

    public void Toggle() {
        if (selected) {
            DeSelect();
        } else {
            Select();
        }
    }

    public bool IsSelected() {
        return selected;
    }

    private void Select() {
        if (manager.CanFocus()) {
            selected = true;
            outline.OutlineWidth = 0;
            rend.material.color = focusedColor;
            //manager.AddInFocus(gameObject);
            manager.AddSelection(GetComponentInParent<Element>());
        }
    }

    private void DeSelect() {
        if (selected) {
            selected = false;
            outline.OutlineWidth = 0;
            if (correct) {
                rend.material.color = correctColor;
            } else {
                rend.material.color = defCol;
            }
            //manager.SubInFocus(gameObject);
            manager.RemoveSelection(GetComponentInParent<Element>());
        }
    }

    public void StartHover() {
        if (manager.CanFocus() || selected)
            outline.OutlineColor = hoverColor;
        else
            outline.OutlineColor = illegalColor;
        outline.OutlineWidth = 3;
    }

    public void EndHover() {
        outline.OutlineColor = focusedColor;
        if (selected) {
            outline.OutlineWidth = 10;
        } else {
            outline.OutlineWidth = 0;
        }
    }
}
