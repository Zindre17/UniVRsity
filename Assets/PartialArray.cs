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

    private Coroutine routine;
    private Coroutine hintRoutine;
    private readonly float interval = .1f;
    private readonly float movementMagnitude = .4f;

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

    private bool active = true;
    public bool Active {
        get { return active; }
        set {
            if(active != value) {
                active = value;
                UpdateElementStatus();
                if (active)
                    hoverable.Enable();
                else
                    hoverable.Disable();
            }
        }
    }

    public SortingElement Get(int index) {
        return array[index];
    }

    private void UpdateElementStatus() {
        foreach (SortingElement e in array) {
            e.Active = active;
            e.InFocus = inFocus;
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

    public void Show() {
        routine = StartCoroutine(ShowAnimation());
    }

    private IEnumerator ShowAnimation() {
        for(int i = 0; i < Size; i++) {
            array[i].gameObject.SetActive(true);
            yield return new WaitForSeconds(interval);
        }
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
        SpawnMissing();
        SetValues();
    }

    private void SetValues() {
        for(int i = 0; i < Size; i++) {
            array[i].Size = original.Get(i + Start).Size;
            array[i].Index = Start + i;
            array[i].transform.position = GetPosition(i);
        }
    }

    private Vector3 GetPosition(int index) {
        float i = Size % 2 == 0 ? 0.5f - Size/ 2 : -Size/ 2;
        return center.position + new Vector3((index + i) * movementMagnitude, 0, 0);
    }


    private void SpawnMissing() {
        if (array == null)
            array = new List<SortingElement>();
        while(array.Count < Size) {
            SortingElement s = Instantiate(elementPrefab, transform).GetComponent<SortingElement>();
            s.gameObject.SetActive(false);
            array.Add(s);
        }
    }

    private void AdjustBox() {
        
        box.transform.localScale = new Vector3(Size/2f, origSize.y, origSize.z);
    }

    public void Hint(int index) {
        array[index].Hint();
    }

    public void HintArray() {
        if(hintRoutine != null) {
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

}
