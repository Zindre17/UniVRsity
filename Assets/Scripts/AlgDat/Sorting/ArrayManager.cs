﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrayManager : MonoBehaviour
{
    public Transform arrayCenter;

    public GameObject elementPrefab;

    private int spawnedElements = 0;
    private int[] sortedArray;
    private int[] unsortedArray;
    private readonly int arrayLength = 7;

    private readonly float movementMagnitude = 0.4f;
    private readonly float interval = .1f;
    public readonly int index = -1;

    private List<SortingElement> arrayToSort;
    private SortingElement stored;
    private SortingElement prevPivot;
    private SortingElement compared1;
    private SortingElement compared2;

    private Coroutine spawnRoutine = null;
    private Coroutine actionRoutine = null;

    private ColorManager cm;
    public Renderer rend;

    public Hoverable hoverable;

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
        if (!inFocus) {
            Color c = rend.material.color;
            rend.material.color = new Color(c.r, c.g, c.b, .3f);
        }
    }

    private bool inFocus = true;
    public bool InFocus {
        get { return inFocus; }
        set {
            if( value != inFocus) {
                inFocus = value;
                UpdateElementStatus();
                UpdateColor();
            }
        }
    }

    public SortingElement Get(int index) {
        return arrayToSort[index];
    }

    private bool active = true;
    public bool Active {
        get { return active; }
        set {
            if (active != value) {
                active = value;
                UpdateElementStatus();
                if (active)
                    hoverable.Enable();
                else
                    hoverable.Disable();
            }
        }
    }

    private void UpdateElementStatus() {
        foreach(SortingElement e in arrayToSort) {
            e.Active = active;
            e.InFocus = inFocus;
        }
    }
    

    public int Size {
        get { return arrayLength; }
    }

    public void New(int[] _array) {
        if (arrayToSort == null)
            arrayToSort = new List<SortingElement>(arrayLength);
        unsortedArray = (int[])_array.Clone();
        sortedArray = (int[])_array.Clone();
        System.Array.Sort(sortedArray);
        Restart();
    }

    public void Restart() {
        transform.localPosition = Vector3.zero;
        Active = true;
        InFocus = true;
        if (compared1 != null) 
            compared1 = compared2 = null;
        if(prevPivot != null) {
            prevPivot.Pivot = false;
            prevPivot = null;
        }
        if(stored != null)
            stored.gameObject.SetActive(false);
        if (spawnedElements < arrayLength)
            SpawnArray();
        if (spawnRoutine != null) 
            StopCoroutine(spawnRoutine);
        if (actionRoutine != null)
            StopCoroutine(actionRoutine);
        spawnRoutine = StartCoroutine(EditArray());
    }

    public void Hint(int index) {
        Get(index).Hint();
    }

    private Coroutine hintRoutine;

    public void Hint() {
        if (hintRoutine != null) {
            StopCoroutine(hintRoutine);
        }
        hintRoutine = StartCoroutine(HintAnimation());
    }

    private IEnumerator HintAnimation() {
        UpdateColor();
        Color def = rend.material.color;
        Color hint = cm.hint;
        yield return new WaitForSeconds(.1f);
        rend.material.color = hint;
        yield return new WaitForSeconds(.3f);
        rend.material.color = def;
        yield return new WaitForSeconds(.3f);
        rend.material.color = hint;
        yield return new WaitForSeconds(.3f);
        rend.material.color = def;
        hintRoutine = null;
    }

    private IEnumerator EditArray() {
        int i;
        for(i = 0; i < spawnedElements; i++) {
            arrayToSort[i].Set(i, unsortedArray[i], sortedArray[i]);
            arrayToSort[i].transform.position = GetPosition(i);
            arrayToSort[i].gameObject.SetActive(false);
        }
        for (i = 0; i < spawnedElements; i++) {
            arrayToSort[i].gameObject.SetActive(true);
            yield return new WaitForSeconds(interval);
        }
        spawnRoutine = null;
    }

    private void SpawnArray() {
        while(spawnedElements < arrayLength) {
            SpawnElement();
        }
    }

    private Vector3 GetPosition(int index) {
        float i = arrayLength % 2 == 0 ? 0.5f - arrayLength / 2 : -arrayLength / 2;
        return arrayCenter.position + new Vector3((index + i) * movementMagnitude, 0, 0);
    }

    private void SpawnElement() {
        //instatiate new sorting element and set correct size and index
        SortingElement e = CreateElement();
        e.Set(spawnedElements, unsortedArray[spawnedElements], sortedArray[spawnedElements]);
        e.transform.position = GetPosition(spawnedElements);

        // add to array of all sorting elements
        arrayToSort.Add(e);

        spawnedElements++;
        e.gameObject.SetActive(false);
    }

    private SortingElement CreateElement() {
        return Instantiate(elementPrefab,transform).GetComponent<SortingElement>();
    }

    
}
