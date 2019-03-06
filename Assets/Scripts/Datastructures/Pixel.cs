using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pixel : MonoBehaviour
{
    private ColorManager colors;
    private Hoverable hoverable;
    private bool legal = true;

    public int index;

    private Renderer rend;
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
    private bool visited = false;
    public bool Visited {
        get { return visited; }
        set {
            if (visited != value) {
                visited = value;
                if (value) {
                    hoverable.Legal = false;
                }
            }
        }
    }

    private bool pattern = false;
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
        rend = GetComponentInChildren<Renderer>();
        hoverable = GetComponentInChildren<Hoverable>();
        colors = ColorManager.instance;
    }

    private void Start() {
        SetColor(dark);
    }

    private void SetColor(bool b) {
        color = b ? colors.darkColor : colors.lightColor;
        UpdateColor();
    }

    private void UpdateColor() {
        Color c = color;
        Color oc = color;
        if (selected) {
            c = colors.selectedColor;
        }if(seed) {
            oc = colors.seedColor;
        } else if (pattern) {
            oc = colors.patternColor;
        } else if (visited) {
            oc = colors.visitedColor;
        }    
        rend.materials[1].color = c;
        rend.materials[2].color = oc;
    }

}
