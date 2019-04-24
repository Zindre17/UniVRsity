using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrays : MonoBehaviour
{
    public ArrayManager array;
    public StorageManager storage;
    public GameObject partialArrayPrefab;
    public GameObject CombinedArrayPrefab;
    public ElementAnimator anim;

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
            if (current == null) {
                array.Active = true;
                array.InFocus = true;
            } else {
                current.Left.Active = true;
                current.Left.InFocus = true;
                current.Right.Active = true;
                current.Right.InFocus = true;
            }
        }
    }
    private Stack<int> copies;
    private Stack<int> stored;
    private Stack<Split> mergedSplits;
    private Stack<CombinedArray> mergedCombined;

    private readonly float spacing = .15f;
    private readonly int size = 7;
    public int Size { get { return size; } }

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

    public void UndoMerge() {
        Split s = mergedSplits.Pop();
        CombinedArray c = mergedCombined.Pop();
        current = s;
        splits.Add(s.Left);
        splits.Add(s.Right);
        layers.Add(s);
        mergeArray = c;
        anim.UndoMergeCompletion(array, splits, c, s);
    }

    private void MergeComplete() {
        splits.Remove(current.Left);
        splits.Remove(current.Right);
        layers.Remove(current);
        if (mergedCombined == null)
            mergedCombined = new Stack<CombinedArray>();
        if (mergedSplits == null)
            mergedSplits = new Stack<Split>();
        Split c = current;
        CombinedArray co = mergeArray;
        mergedSplits.Push(c);
        mergedCombined.Push(co);
        mergeArray = null;
        if (layers.Count > 0)
            current = layers[layers.Count - 1];
        else
            current = null;
        anim.MergeComplete(array, splits, c, co);
        
    }

    public void Complete() {
        array.Active = false;
        array.InFocus = true;
        //anim.Stop();
        //storage.Stop();
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
            s = pa.GetExpansion();
            if (end < pa.Size)
                s.InFocus = false;
            else
                s.InFocus = true;
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

    public void Unsplit() {
        splits.Remove(current.Right);
        splits.Remove(current.Left);
        if (layers.Count > 1)
            Current = layers[layers.Count - 2];
        else
            Current = null;
        anim.Unsplit(array,layers);
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

    public void Unmerge() {
        anim.Unmerge(current, mergeArray);
    }
    public void Merge(MergeAction action) {
        // move the current split apart from eachother
        // spawn a new blank array between combined size
        // set the first element in both the splits as infocus
        // 
        mergeArray = Instantiate(CombinedArrayPrefab, transform).GetComponent<CombinedArray>();
        int s = current.Left.Size + current.Right.Size;
        mergeArray.Init(s, current.Left.Start);
        //Vector3 pos = (current.Right.transform.position - current.Left.transform.position) / 2f + current.Left.transform.position;
        Vector3 pos = new Vector3(current.Left.transform.position.x + current.Left.Size / 4f + spacing, current.Left.transform.position.y, current.Left.transform.position.z);
        mergeArray.transform.position = pos;
        mergeArray.gameObject.SetActive(false);
        anim.Merge(current, mergeArray);
    }

    public void Swap(SwapAction action, bool reverse = false) {
        SortingElement s1, s2;
        s1 = array.Get(action.index1);
        s2 = array.Get(action.index2);
        anim.Swap(s1, s2, reverse);
    }

    public void Pivot(PivotAction action, bool reverse = false) {
        anim.Pivot(array.Get(action.pivotIndex),reverse);
    }

    public void Store(StoreAction action, bool reverse = false) {
        SortingElement s = array.Get(action.index);
        if (stored == null)
            stored = new Stack<int>();
        if (reverse) {
            stored.Pop();
            int prev = stored.Count == 0 ? -1 : stored.Peek();
            anim.Unstore(s, prev, storage.Get(), storage.GetCenter());
        } else {
            stored.Push(s.Size);
            anim.Store(storage.GetCenter(), storage.Get(), s);
        }
    }

    public void CopyTo(MoveAction action, bool reverse = false) {
        SortingElement s1, s2;
        if (copies == null)
            copies = new Stack<int>();
        if (action.array < 0) {
            if (action.source == -1)
                s1 = storage.Get();
            else
                s1 = array.Get(action.source);
            s2 = array.Get(action.target);
        } else {
            s1 = splits[action.array].Get(action.source);
            if (reverse)
                s2 = mergeArray.Get(mergeArray.Replaced - 1);
            else
                s2 = mergeArray.Replace();
        }
        if (reverse) {
            anim.UndoCopyTo(s1, s2, copies.Pop(),mergeArray);
        } else {
            copies.Push(s2.Size);
            anim.CopyTo(s1, s2);
        }
    }
}
