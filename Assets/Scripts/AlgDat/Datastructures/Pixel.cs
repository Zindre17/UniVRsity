using System;
using System.Collections;
using UnityEngine;

public class Pixel : MonoBehaviour
{
    public int index;
    private ColorManager colorManager;
    public Renderer rend;
    public Transform surface;
    private bool selected = false;
    public bool Selected {
        get { return selected; }
        set {
            if (selected != value ) {
                selected = value;
                UpdateColor();
            }
        }
    }
    private bool next = false;
    public bool Next {
        get { return next; }
        set {
            if (next != value) {
                next = value;
            }
        }
    }

    private bool seed = false;
    public bool Seed {
        get { return seed; }
        set {
            if (seed != value) {
                seed = value;
                UpdateColor();
            }
        }
    }

    private Color color;
    private bool dark = false;
    public bool Dark {
        get { return dark; }
        set {
            if (dark != value) {
                dark = value;
                SetColor(dark);
            }
        }
    }

    private void Awake() {
        colorManager = ColorManager.instance;
    }

    private void Start() {
        SetColor(dark);
    }

    private void SetColor(bool b) {
        color = b ? colorManager.black: colorManager.white;
        UpdateColor();
    }

    public void Hint(Action function = null) {
        StartCoroutine(HintAnimation(function:function));
    }

    public Vector3 GetSurface() {
        return surface.position;
    }

    private IEnumerator HintAnimation(Action function = null) {
        Color hint = colorManager.hint;
        Color def = selected ? colorManager.selected: rend.materials[0].color;
        yield return new WaitForSeconds(.1f);
        rend.materials[1].color = hint;
        yield return new WaitForSeconds(.3f);
        rend.materials[1].color = def;
        yield return new WaitForSeconds(.3f);
        rend.materials[1].color = hint;
        yield return new WaitForSeconds(.3f);
        rend.materials[1].color = def;
        if (function != null) function();
    }

    private void UpdateColor() {
        Color c = color;
        Color oc = color;
        Color ic = color;
        if (selected) {
            oc = colorManager.selected;
        }
        if (next) {
            ic = colorManager.next;
        }else if (seed) {
            ic = colorManager.seed;
        }    
        rend.materials[0].color = c;
        rend.materials[1].color = oc;
        rend.materials[2].color = ic;
    }

}
