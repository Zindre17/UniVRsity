using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PartialArray : Selectable
{
    public Transform center;
    public TextMeshPro text;
    public GameObject elementPrefab;
    public GameObject box;

    [HideInInspector]
    public ArrayManager original;

    private List<SortingElement> array;
    private SortingElement expansion;

    private Vector3 origSize;

    public bool ParentLeft { get; set; }
    public bool Left { get; set; }

    public int Start { get; private set; }
    public int End { get; private set; }
    public int Size { get; private set; }

    public Vector3 MergePos { get; set; }
    private Vector3 startPos;
    public Vector3 StartPos {
        get { return startPos; }
        set {
            startPos = value;
            transform.position = startPos;
        }
    }

    private Coroutine routine;
    private readonly float interval = .1f;
    private readonly float movementMagnitude = .4f;

    private bool inFocus = true;
    public bool InFocus {
        get { return inFocus; }
        set {
            if(inFocus != value) {
                inFocus = value;
                UpdateElementStatus();
                UpdateColor();
            }
        }
    }

    private void OnDestroy() {
        if (routine != null)
            StopCoroutine(routine);
    }

    public SortingElement Get(int index) {
        if (index >= Size)
            return expansion;
        return array[index];
    }

    private void UpdateElementStatus() {
        foreach (SortingElement e in array) {
            e.Active = active;
            e.InFocus = inFocus;
        }
    }

    internal override void UpdateColor() {
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

    public void Show() {
        routine = StartCoroutine(ShowAnimation());
    }

    private IEnumerator ShowAnimation() {
        for(int i = 0; i < Size; i++) {
            array[i].gameObject.SetActive(true);
            yield return new WaitForSeconds(interval);
        }
        routine = null;
    }

    public void Init(ArrayManager _array, int _start, int _end, int _index) {
        original = _array;
        Index = _index;
        Start = _start;
        End = _end;
        Size = End - Start;
        text.text = "A[" + Start + ":" + (End-1) + "]";
        origSize = box.transform.localScale;
        Adjust();
    }

    private Stack<int> prevValues;
    public void EditValues(int[] values) {
        int i = 0;
        if (prevValues == null)
            prevValues = new Stack<int>();
        foreach (int num in values) {
            if (num == -1) {
                prevValues.Push(array[i].Size);
            } else {
                array[i].Size = num;
                prevValues.Push(num);
            }
            i++;
            if (i >= Size)
                break;
        }
    }

    public void RevertValues() {
        for (int i = Size-1; i > -1; i--) {
            int num = prevValues.Pop();
            array[i].Size = num;
        }
    }

    public SortingElement GetExpansion() {
        return expansion;
    }

    public void Unexpand() {
        expansion.gameObject.SetActive(false);
        text.text = "A[" + Start + ":" + (End - 1) + "]";
        routine = StartCoroutine(UnexpansionAnimation());
    }

    public void Expand() {
        text.text = Index % 2 == 0 ? "L" : "R";
        expansion.gameObject.SetActive(true);
        routine = StartCoroutine(ExpansionAnimation());
    }

    private IEnumerator UnexpansionAnimation() {
        float prevTime = Time.time;
        float elapsed = 0f;
        float percent;
        float duration = .5f;
        List<Vector3> starts = new List<Vector3>(Size);
        List<Vector3> paths = new List<Vector3>(Size);
        Vector3 boxStart = box.transform.localScale;
        Vector3 addition = new Vector3(.5f, 0, 0);
        int i;
        for (i = 0; i < Size; i++) {
            starts.Add(array[i].transform.localPosition);
            paths.Add(GetPosition(i,false) - starts[i]);
        }
        while (elapsed < duration) {
            percent = elapsed / duration;
            for (i = 0; i < Size; i++) {
                array[i].transform.localPosition = starts[i] + paths[i] * percent;
            }
            box.transform.localScale = boxStart - addition * percent;
            float time = Time.time;
            elapsed += time - prevTime;
            prevTime = time;
            yield return null;
        }
        box.transform.localScale = boxStart - addition;
        for (i = 0; i < Size; i++) {
            array[i].transform.localPosition = starts[i] + paths[i];
            array[i].InFocus = true;
        }
        routine = null;
        EventManager.PartialActionComplete();
    }

    private IEnumerator ExpansionAnimation() {
        float prevTime = Time.time;
        float elapsed = 0f;
        float percent;
        float duration = .5f;
        List<Vector3> starts = new List<Vector3>(Size);
        List<Vector3> paths = new List<Vector3>(Size);
        Vector3 start = expansion.transform.localScale;
        Vector3 boxStart = box.transform.localScale;
        Vector3 addition = new Vector3(.5f, 0, 0);
        expansion.Size = 20;
        int i;
        for (i = 0; i < Size; i++) {
            starts.Add(array[i].transform.localPosition);
            paths.Add(GetPosition(i, true) - starts[i]);
        }
        while(elapsed < duration) {
            percent = elapsed / duration;
            for(i = 0; i < Size; i++) {
                array[i].transform.localPosition = starts[i] + paths[i] * percent;
            }
            expansion.transform.localScale = start * percent;
            box.transform.localScale = boxStart + addition * percent;
            float time = Time.time;
            elapsed += time - prevTime;
            prevTime = time;
            yield return null;
        }
        box.transform.localScale = boxStart + addition;
        expansion.transform.localScale = start;
        for(i = 0; i< Size; i++) {
            array[i].transform.localPosition = starts[i] + paths[i];
        }
        expansion.Size = 20;
        routine = null;
        EventManager.PartialActionComplete();
    }

    private void Adjust() {
        AdjustArray();
        AdjustBox();
    }

    private void AdjustArray() {
        SpawnMissing();
        SetValues();
    }

    private void SetValues() {
        for(int i = 0; i < Size; i++) {
            array[i].Size = original.Get(i + Start).Size;
            array[i].Index = i;
            array[i].Parent = Index;
            array[i].transform.localPosition = GetPosition(i, false);
        }
        
    }

    private Vector3 GetPosition(int index, bool expanded) {
        int s = Size + (expanded ? 1 : 0);
        float i = s % 2 == 0 ? 0.5f - s/ 2 : -s/ 2;
        return center.localPosition + new Vector3((index + i) * movementMagnitude, 0, 0);
    }


    private void SpawnMissing() {
        if (array == null)
            array = new List<SortingElement>(Size);
        while(array.Count < Size) {
            SortingElement s = Instantiate(elementPrefab, transform).GetComponent<SortingElement>();
            s.gameObject.SetActive(false);
            array.Add(s);
        }
        if (expansion == null) {
            SortingElement e = Instantiate(elementPrefab, transform).GetComponent<SortingElement>();
            e.gameObject.SetActive(false);
            expansion = e;
            expansion.Size = 20;
            expansion.Parent = Index;
            expansion.Index = Size;
            expansion.transform.localPosition = GetPosition(array.Count, true);
        }
    }

    private void AdjustBox() {
        
        box.transform.localScale = new Vector3(Size/2f, origSize.y, origSize.z);
    }

    public void Hint(int index) {
        if (index >= Size)
            expansion.Hint();
        else
            array[index].Hint();
    }
    
    internal override void SetActive(bool a) {
        UpdateElementStatus();
        if (active)
            hoverable.Enable();
        else
            hoverable.Disable();
    }

    internal override void SetSelected(bool s) {
        UpdateColor();
    }
}
