using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Hoverable))]
public class SortingElement : Selectable {

    private bool correct = false;

    private int size;
    public int Size {
        get { return size; }
        set {
            size = value;
            transform.localScale = new Vector3(1, Size, 1);
            correct = size == goal;
            UpdateColor();
        }
    }

    private bool inFocus = true;
    public bool InFocus {
        get { return inFocus; }
        set {
            if(value != inFocus) {
                inFocus = value;
                hoverable.Legal = inFocus;
                UpdateColor();
            }
        }
    }

   

    private int goal;

    public void Set(int i, int v, int g) {
        Index = i;
        goal = g;
        Size = v;
        Selected = false;
        Pivot = false;
    }

    public float movementMagnitude = 0.4f;

    private bool pivot;
    public bool Pivot {
        get { return pivot; }
        set {
            if(value != pivot) {
                pivot = value;
                UpdateColor();
            }
        }
    }

    internal override void UpdateColor() {
        if (rend == null)
            return;
        if (cm == null)
            cm = ColorManager.instance;
        Color color;
        if (selected) {
            color = cm.selected;
        } else if (pivot) {
            color = cm.pivot;
        } else if (correct) {
            color = cm.correct;
        } else {
            color = cm.element;
        }

        if (!inFocus) {
            color = new Color(color.r, color.g, color.b, 0.3f);
        }
        rend.material.color = color;
    }

    internal override void SetActive(bool a) {
        if (active) {
            hoverable.Enable();
        } else {
            hoverable.Disable();
        }
    }

    internal override void SetSelected(bool s) {
        UpdateColor();
    }
   
}
