﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortingElement : Selectable {

    private Stack<int> prevValues;
    public TMPro.TextMeshPro label;
    public GameObject cube;
    public Renderer WallBehindText;
    public Hoverable hoverable;
    private bool correct = false;

    private int size;
    public int Size {
        get { return size; }
        set {
            size = value;
            cube.transform.localScale = new Vector3(1, Size, 1);
            correct = size == goal;
            UpdateColor();
        }
    }

    public void SetSize(int value) {
        if (prevValues == null)
            prevValues = new Stack<int>();
        prevValues.Push(size);
        Size = value;
    }

    public void Revert() {
        Size = prevValues.Pop();
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

    public void Set(int i, int v, int g, int p) {
        Index = i;
        label.text = string.Format("A[{0}]", Index);
        goal = g;
        Size = v;
        Selected = false;
        Pivot = false;
        Parent = p;
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
        WallBehindText.material.color = color;
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
