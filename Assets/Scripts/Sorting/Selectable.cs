using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour {

    public Renderer rend;
    private Color defCol;

    private bool inFocus = true;
    public bool InFocus {
        get { return inFocus; }
        set {
            if (value != inFocus) {
                inFocus = value;
                UpdateColor();
            }
        }
    }

    private bool correct = false;
    public bool Correct {
        get { return correct; }
        set {
            correct = value;
            UpdateColor();
        }
    }
    private bool selected = false;
    public bool Selected {
        get {
            return selected;
        }
        set {
            if (selected != value) {
                if (value && !inFocus)
                    return;
                selected = value;
                HandleChange();
                UpdateColor();
            }
        }
    }

    private bool pivot = false;
    public bool Pivot {
        get { return pivot; }
        set {
            if (pivot == value) return;
            pivot = value;
            UpdateColor();
        }
    }

    public Color correctColor;
    public Color selectedColor;
    public Color pivotColor;

    public delegate void IsSelected();
    public static event IsSelected OnSelected;

    public delegate void IsDeselected();
    public static event IsDeselected OnDeselected;

    private void Awake() {
        if (rend == null)
            rend = GetComponentInChildren<Renderer>();
        if(rend!=null)
            defCol = rend.material.color;
    }

   

    private void HandleChange() {
        if (selected && OnSelected != null)
            OnSelected();
        else if (OnDeselected != null)
            OnDeselected();
    }

    private void UpdateColor() {
        if (rend == null)
            return;

        Color color;
        if (selected) {
            color = selectedColor;
        } else if (pivot) {
            color = pivotColor;
        } else if (correct) {
            color = correctColor;
        } else {
            color = defCol;
        }
        
        if (inFocus) {
            color = new Color(color.r, color.g, color.b, 1);
        } else {
            color = new Color(color.r, color.g, color.b, 0.3f);
        }
        rend.material.color = color;
    }
}
