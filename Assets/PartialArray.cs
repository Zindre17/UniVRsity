using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PartialArray : MonoBehaviour
{
    public Transform center;
    public TextMeshPro text;
    public GameObject elementPrefab;
    public GameObject box;
    public Renderer rend;
    public Hoverable hoverable;

    [HideInInspector]
    public ArrayManager original;

    private List<SortingElement> array;

    private Vector3 origSize;

    private ColorManager cm;

    public int Start { get; private set; }
    public int End { get; private set; }
    public int Size { get; private set; }
    public int Index { get; private set; }

    private bool selected = false;
    public bool Selected {
        get { return selected; }
        set {
            if(selected != value) {
                selected = value;
                UpdateColor();
            }
        }
    }

    private void UpdateColor() {
        if (cm == null)
            cm = ColorManager.instance;
        if (selected)
            rend.material.color = cm.selected;
        else
            rend.material.color = cm.box;
    }

    public void Init(ArrayManager _array, int _start, int _end, int _index) {
        original = _array;
        Index = _index;
        Start = _start;
        End = _end;
        Size = End - Start;
        origSize = box.transform.localScale;
        Adjust();
    }

    private void Adjust() {
        AdjustArray();
        AdjustBox();
    }

    private void AdjustArray() {
        if (array == null)
            array = new List<SortingElement>();

    }

    private void AdjustBox() {
        
        box.transform.localScale = new Vector3(origSize.x / original.Size * Size, origSize.y, origSize.z);
    }
}
