using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrays : MonoBehaviour
{
    public ArrayManager array;
    public StorageManager storage;
    public GameObject partialArrayPrefab;
    public ElementAnimator anim;

    private List<PartialArray> splits;
    private List<Split> layers;
    private Split current;

    private readonly int size = 7;
    public int Size { get { return size; } }

    private int nextSplitIndex = 0;

    private Vector3 splitPath = Vector3.forward*.5f;

    public SortingElement Get(int index) {
        return array.Get(index);
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

    public void New() {
        anim.Stop();
        GenerateRandomArray();
        array.New(Array);
    }

    public void Restart() {
        anim.Stop();
        array.Restart();
    }

    public void Hint(int index) {

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
        // main array
        if (action.array == -1) {
            int mid = Mathf.FloorToInt(array.Size / 2f);
            a.Init(array, 0, mid - 1, nextSplitIndex);
            nextSplitIndex++;
            b.Init(array, mid, array.Size - 1, nextSplitIndex);
            nextSplitIndex++;
            Vector3 inheritSpot = array.transform.position;
            a.transform.position = inheritSpot;
            b.transform.position = inheritSpot;
        } 
        //partial array
        else {
            PartialArray toSplit = splits[action.array];
            int mid = Mathf.FloorToInt(toSplit.Size / 2f);
            a.Init(array, toSplit.Start, toSplit.Start + mid - 1, nextSplitIndex);
            nextSplitIndex++;
            b.Init(array, toSplit.Start + mid, toSplit.End, nextSplitIndex);
            nextSplitIndex++;
            a.transform.position = toSplit.transform.position;
            b.transform.position = toSplit.transform.position;
        }
        splits.Add(a);
        splits.Add(b);
        Split split = new Split(a, b);
        current = split;
        anim.Split(array, split, layers);
    }

    public void Merge(MergeAction action) {
        //add inf to end of both active arrays
    }

    public string Compare(CompareAction action) {
        SortingElement s1, s2;
        if (action.index1 == -1)
            s1 = storage.Get();
        else
            s1 = array.Get(action.index1);
        if (action.index2 == -1)
            s2 = storage.Get();
        else
            s2 = array.Get(action.index2);

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
        if (action.source == -1)
            s1 = storage.Get();
        else
            s1 = array.Get(action.source);
        s2 = array.Get(action.target);
        anim.CopyTo(s1, s2);
    }
}
