﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombinedArray : MonoBehaviour
{
    public GameObject box;
    public Transform center;
    public TMPro.TextMeshPro text;

    public GameObject elementPrefab;
    public GameObject emptyPrefab;

    private List<SortingElement> elements;
    private List<EmptyElement> e;

    private int size;
    private int replaced = 0;
    public int Replaced {
        get { return replaced; }
    }
    private int start;

    public int Size { get { return size; } }

    private Vector3 pos;
    public Vector3 Pos {
        get { return pos; }
        set {
            pos = value;
            transform.position = pos;
        }
    }

    public bool Complete()
    {
        return replaced == size;
    }

    public void Init(int _size, int _start) {
        size = _size;
        start = _start;
        elements = new List<SortingElement>(size);
        e = new List<EmptyElement>(size);
        box.transform.localScale = new Vector3(size / 2f, 1, 1);
        for(int i = 0; i < size; i++) {
            SortingElement s = Instantiate(elementPrefab, transform).GetComponent<SortingElement>();
            s.gameObject.SetActive(false);
            s.Index = start + i;
            elements.Add(s);
            EmptyElement l = Instantiate(emptyPrefab, transform).GetComponent<EmptyElement>();
            e.Add(l);
            s.transform.position = l.transform.position = GetPos(i);
        }
    }

    public EmptyElement GetEmpty() {
        return e[replaced];
    }

    public SortingElement Get(int index) {
        return elements[index];
    }

    public SortingElement Get() {
        if(replaced < size)
            return elements[replaced];
        return null;
    }

    private Vector3 GetPos(int index) {
        float i = size % 2 == 0 ? 0.5f - size / 2 : -size/ 2;
        return center.position + new Vector3((index + i) * .4f, 0, 0);
    }

    public void Hint() {
        e[replaced].Hint();
    }
    public void Undo() {
        replaced--;
        SortingElement s = elements[replaced];
        s.gameObject.SetActive(false);
        e[replaced].gameObject.SetActive(true);
    }

    public SortingElement Replace() {
        if (!(replaced < size)) return null;
        SortingElement s = elements[replaced];
        s.gameObject.SetActive(true);
        e[replaced].gameObject.SetActive(false);
        replaced++;
        //if (replaced == size) EventManager.MergeComplete();
        return s;
    }
}
