﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrays : MonoBehaviour
{
    public ArrayManager array;
    public StorageManager storage;
    public GameObject partialArrayPrefab;
    public GameObject CombinedArrayPrefab;
    public ElementAnimator anim;
    public Renderer backWall;

    private List<PartialArray> splits;
    private List<Split> layers;
    private CombinedArray mergeArray;
    private Split current;
    private Split Current {
        get { return current; }
        set {
            if (current != null) {
                current.Left.Active = false;
                current.Left.InFocus = false;
                current.Right.Active = false;
                current.Right.InFocus = false;
            }
            current = value;
            current.Left.Active = true;
            current.Left.InFocus = true;
            current.Right.Active = true;
            current.Right.InFocus = true;
        }
    }

    private readonly float spacing = .15f;
    private readonly int size = 7;
    public int Size { get { return size; } }

    private Vector3 splitPath = Vector3.forward*.5f;

    public SortingElement Get(int index) {
        return array.Get(index);
    }

    public Selectable GetElement(int index, int _array = -2) {
        Selectable s;
        if (index == -1)
            s = storage.Get();
        else if (index == -2)
            s = mergeArray.GetEmpty();
        else {
            if (_array < 0)
                s = array.Get(index);
            else
                s = splits[_array].Get(index);
        }
        return s;
    }

    public Selectable GetArray(int index) {
        Selectable s;
        if (index == -1)
            s = array;
        else
            s = splits[index];
        return s;
    }

    public int[] Array {
        get;
        private set;
    }

    private void GenerateRandomArray() {
        Array = new int[size];
        for (int i = 0; i < size; i++) {
            int num = UnityEngine.Random.Range(1, 16);
            Array[i] = num;
        }
    }

    private void Awake() {
        EventManager.OnArrayInFocusChanged += FocusChanged;
        EventManager.OnMergeComplete += MergeComplete;
    }

    private void OnDestroy() {
        EventManager.OnArrayInFocusChanged -= FocusChanged;
        EventManager.OnMergeComplete -= MergeComplete;
    }

    private void MergeComplete() {
        splits.Remove(current.Left);
        splits.Remove(current.Right);
        layers.Remove(current);
        Split c = current;
        CombinedArray co = mergeArray;
        mergeArray = null;
        if (layers.Count > 0)
            current = layers[layers.Count - 1];
        else
            current = null;
        anim.MergeComplete(array, splits, c, co);
        
    }

    public void Complete() {
        int i;
        if(current == null) {
            FocusChanged(-1, 0, array.Size);
            for(i = 0; i < array.Size; i++) {
                array.Get(i).Active = false;
            }
        } else {
            FocusChanged(current.Left.Index, 0, current.Left.Size);
            for (i = 0; i < current.Left.Size; i++) {
                current.Left.Get(i).Active = false;
            }
            FocusChanged(current.Right.Index, 0, current.Right.Size);
            for (i = 0; i < current.Right.Size; i++) {
                current.Right.Get(i).Active = false;
            }
        }
        storage.Stop();
    }

    private void FocusChanged(int _array, int start, int end) {
        SortingElement s;
        int i;
        if (_array == -1) {
            for(i = 0; i < array.Size; i++) {
                s = array.Get(i);
                if (i < start || i > end)
                    s.InFocus = false;
                else
                    s.InFocus = true;
            }
        } else {
            PartialArray pa = splits[_array];
            for (i = 0; i < pa.Size; i++) {
                s = pa.Get(i);
                if (i < start || i > end)
                    s.InFocus = false;
                else
                    s.InFocus = true;
            }
        }
    }

    public void New() {
        anim.Stop();
        GenerateRandomArray();
        array.New(Array);
        array.Active = true;
        array.InFocus = true;
        storage.Stop();
    }

    public void Restart() {
        anim.Stop();
        array.Restart();
        storage.Stop();
        array.Active = true;
        array.InFocus = true;
        if (splits != null) {
            layers.Clear();
            for (int i = splits.Count - 1; i > -1; i--) {
                PartialArray p = splits[i];
                splits.Remove(p);
                Destroy(p.gameObject);
            }
        }
        if (current != null) {
            Destroy(current.Left.gameObject);
            Destroy(current.Right.gameObject);
            current = null;
        }
        if (mergeArray != null) {
            Destroy(mergeArray.gameObject);
            mergeArray = null;
        }
    }

    public void Hint(GameAction a) {
        switch (a.type) {
            case GameAction.GameActionType.Compare:
                CompareAction c = (CompareAction)a;
                Hint(c.index1, c.array1);
                Hint(c.index2, c.array2);
                break;
            case GameAction.GameActionType.Move:
                MoveAction m = (MoveAction)a;
                Hint(m.source, m.array);
                if (m.target == -2)
                    mergeArray.Hint();
                else
                    Hint(m.target);
                break;
            case GameAction.GameActionType.Pivot:
                PivotAction p = (PivotAction)a;
                Hint(p.pivotIndex);
                break;
            case GameAction.GameActionType.Store:
                StoreAction t = (StoreAction)a;
                Hint(t.index);
                break;
            case GameAction.GameActionType.Swap:
                SwapAction s = (SwapAction)a;
                Hint(s.index1);
                Hint(s.index2);
                break;
            case GameAction.GameActionType.Split:
                SplitAction sa = (SplitAction)a;
                HintArray(sa.array);
                break;
            case GameAction.GameActionType.Merge:
                MergeAction ma = (MergeAction)a;
                HintArray(ma.a1);
                HintArray(ma.a2);
                break;
        }
    }

    public void Hint(int index, int _array = -2) {
        if(_array == -2) {
            if (index == -1)
                storage.Hint();
            else {
                array.Hint(index);
            }
        } else {
            splits[_array].Hint(index);
        }
    }

    public void HintArray(int index) {
        if (index == -1)
            array.Hint();
        else {
            splits[index].Hint();
        }
    }

    public void Split(SplitAction action) {

        //move every layer back and current arrays to layers

        if (splits == null)
            splits = new List<PartialArray>();
        if (layers == null)
            layers = new List<Split>();
        //spawn two new partial arrays from that
        PartialArray a, b;
        a = Instantiate(partialArrayPrefab,transform).GetComponent<PartialArray>();
        b = Instantiate(partialArrayPrefab,transform).GetComponent<PartialArray>();
        Vector3 midPos, leftOffset, rightOffset;
        // main array
        if (action.array == -1) {
            Color c = backWall.material.color;
            backWall.material.color = new Color(c.r, c.g, c.b, .6f);
            int mid = Mathf.CeilToInt(array.Size / 2f);
            a.Init(array, 0, mid, splits.Count);
            array.Active = false;
            array.InFocus = false;
            b.Init(array, mid, array.Size , splits.Count+1);
            midPos = array.transform.position;
            leftOffset = new Vector3(-array.Size/4f + a.Size/4f - spacing, 0f, 0f);
            rightOffset = new Vector3(array.Size/4f - b.Size/4f + spacing, 0f, 0f);
            a.transform.position = midPos + leftOffset;
            b.transform.position = midPos + rightOffset;
        } 
        //partial array
        else {
            PartialArray toSplit = splits[action.array];
            int mid = Mathf.CeilToInt(toSplit.Size / 2f);
            a.Init(array, toSplit.Start, toSplit.Start + mid, splits.Count);
            b.Init(array, toSplit.Start + mid, toSplit.End, splits.Count +1);
            midPos = toSplit.transform.position;
            leftOffset = new Vector3(-toSplit.Size/4f + a.Size/4f - spacing, 0f, 0f);
            rightOffset = new Vector3(toSplit.Size/4f - b.Size/4f + spacing, 0f, 0f);
            a.transform.position = midPos + leftOffset;
            b.transform.position = midPos + rightOffset;
        }
        splits.Add(a);
        splits.Add(b);
        Split split = new Split(a, b);
        Current = split;
        layers.Add(current);
        anim.Split(array, layers);
    }

    public void Merge(MergeAction action) {
        // move the current split apart from eachother
        // spawn a new blank array between combined size
        // set the first element in both the splits as infocus
        // 
        mergeArray = Instantiate(CombinedArrayPrefab, transform).GetComponent<CombinedArray>();
        int s = current.Left.Size + current.Right.Size;
        mergeArray.Init(s, current.Left.Start);
        Vector3 pos = (current.Right.transform.position - current.Left.transform.position) / 2f + current.Left.transform.position;
        mergeArray.transform.position = pos;
        mergeArray.gameObject.SetActive(false);
        anim.Merge(current, mergeArray);
    }

    public string Compare(CompareAction action) {
        SortingElement s1, s2;
        if(action.array1 < 0) {
            if (action.index1 == -1)
                s1 = storage.Get();
            else
                s1 = array.Get(action.index1);
        } else {
            s1 = splits[action.array1].Get(action.index1);
        }

        if(action.array2 < 0) {
            if (action.index2 == -1)
                s2 = storage.Get();
            else
                s2 = array.Get(action.index2);
        } else {
            s2 = splits[action.array2].Get(action.index2);
        }
        anim.Compare(s1, s2);

        if (s1.Index == -1) {
            if (s1.Size > s2.Size) {
                return string.Format("stored > A[{0}]", s2.Index);
            } else if (s1.Size == s2.Size) {
                return string.Format("stored == A[{0}]", s2.Index);
            } else {
                return string.Format("stored < A[{0}]", s2.Index);
            }
        } else if (s2.Index == -1) {
            if (s2.Size > s1.Size) {
                return string.Format("stored > A[{0}]", s1.Index);
            } else if (s1.Size == s2.Size) {
                return string.Format("stored == A[{0}]", s1.Index);
            } else {
                return string.Format("stored < A[{0}]", s1.Index);
            }
        } else {
            if (s1.Size > s2.Size) {
                return string.Format("A[{0}] > A[{1}]", s1.Index, s2.Index);
            } else if (s1.Size == s2.Size) {
                return string.Format("A[{0}] == A[{1}]", s1.Index, s2.Index);
            } else {
                return string.Format("A[{0}] < A[{1}]", s1.Index, s2.Index);
            }
        }
    }

    public void Swap(SwapAction action) {
        SortingElement s1, s2;
        s1 = array.Get(action.index1);
        s2 = array.Get(action.index2);
        anim.Swap(s1, s2);
    }

    public void Pivot(PivotAction action) {
        anim.Pivot(array.Get(action.pivotIndex));
    }

    public void Store(StoreAction action) {
        anim.Store(storage.GetCenter(), storage.Get(), array.Get(action.index));
    }

    public void CopyTo(MoveAction action) {
        SortingElement s1, s2;
        if (action.array < 0) {
            if (action.source == -1)
                s1 = storage.Get();
            else
                s1 = array.Get(action.source);
            s2 = array.Get(action.target);
        } else {
            s1 = splits[action.array].Get(action.source);
            s2 = mergeArray.Replace();
        }
        anim.CopyTo(s1, s2);
    }
}
