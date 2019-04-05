using UnityEngine;

[RequireComponent(typeof(Hoverable))]
public class SortingElement : MonoBehaviour {

    public Renderer rend;
    private ColorManager colorManager;

    public int Index { get; set; }

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

    private bool selected;
    public bool Selected {
        get { return selected; }
        set {
            if (selected != value) {
                selected = value;
                UpdateColor();
            }
        }
    }

    private bool inFocus = true;
    public bool InFocus {
        get { return inFocus; }
        set {
            if(value != inFocus) {
                inFocus = value;
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

    public Hoverable hoverable;

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

    private void UpdateColor() {
        if (rend == null)
            return;
        if (colorManager == null)
            colorManager = ColorManager.instance;
        Color color;
        if (selected) {
            color = colorManager.selected;
        } else if (pivot) {
            color = colorManager.pivot;
        } else if (correct) {
            color = colorManager.correct;
        } else {
            color = colorManager.element;
        }

        if (inFocus) {
            color = new Color(color.r, color.g, color.b, 1);
        } else {
            color = new Color(color.r, color.g, color.b, 0.3f);
        }
        rend.material.color = color;
    }
}
