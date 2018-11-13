using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewSelectable : MonoBehaviour {

    public Renderer rend;
    private Color defCol;

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
            if (selected == value) return;
            selected = value;
            HandleChange();
            UpdateColor();
        }
    }

    public Color correctColor;
    public Color selectedColor;

    public delegate void IsSelected();
    public static event IsSelected OnSelected;

    public delegate void IsDeselected();
    public static event IsDeselected OnDeselected;

    private void Awake() {
        if(rend == null)
            rend = GetComponent<Renderer>();
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
        if (selected) {
            rend.material.color = selectedColor;
            return;
        }else if (correct) {
            rend.material.color = correctColor;
        } else {
            rend.material.color = defCol;
        }
    }
}
