using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class Comparison : MonoBehaviour
{

    public GameObject Left;
    public GameObject Right;
    private Vector3 leftOrigin, rightOrigin;
    public TextMeshPro lText;
    public TextMeshPro rText;
    public TextMeshPro comparison;

    private Coroutine routine;

    private Stack<SortingElement> prevCompares;
    private Stack<int> when;

    public void Clear() {
        Left.SetActive(false);
        Right.SetActive(false);
        lText.text = "";
        rText.text = "";
        comparison.text = "";
    }
    
    public void Compare(SortingElement s1, SortingElement s2, int step) {
        if (prevCompares == null) {
            prevCompares = new Stack<SortingElement>();
            when = new Stack<int>();
            leftOrigin = Left.transform.position;
            rightOrigin = Right.transform.position;
        }
        prevCompares.Push(s1);
        prevCompares.Push(s2);
        when.Push(step);
        routine = StartCoroutine(CompareAnimation(s1, s2));
    }

    public void Reverse(int step) {
        if (prevCompares == null || when == null) return;
        if (when.Count == 0) return;
        SortingElement s1, s2;
        s2 = prevCompares.Pop();
        s1 = prevCompares.Pop();
        when.Pop();
        routine = StartCoroutine(ReverseAnimation(s1, s2,step));
    }


    public void LoadPrev(int step) {
        if (when.Count != 0)
        {
            if (when.Peek() == step - 2)
            {
                Left.SetActive(true);
                Right.SetActive(true);
                SortingElement s1, s2;
                s2 = prevCompares.Pop();
                s1 = prevCompares.Pop();
                SetState(s1, s2);
                prevCompares.Push(s1);
                prevCompares.Push(s2);
                Left.transform.position = leftOrigin;
                Right.transform.position = rightOrigin;
            }
            else
                Clear();
        }else
            Clear();
    }

    private void SetState(SortingElement s1, SortingElement s2) {
        string c = "{0} {1} {2}";
        string l, m, r;
        if (s1.Size < s2.Size)
            m = "<";
        else if (s1.Size == s2.Size)
            m = "==";
        else
            m = ">";
        if (s1.Index == -1)
            l = "stored";
        else {
            if (s1.Parent < 0) {
                l = "A[" + s1.Index + "]";
            } else {
                l = s1.Parent % 2 == 0 ? "L[" + s1.Index + "]" : "R[" + s1.Index + "]";
            }
        }
        if (s2.Index == -1)
            r = "stored";
        else {
            if (s2.Parent < 0) {
                r = "A[" + s2.Index + "]";
            } else {
                r = s2.Parent % 2 == 0 ? "L[" + s2.Index + "]" : "R[" + s2.Index + "]";
            }
        }
        comparison.text = string.Format(c, l, m, r);
        if (l.Equals("stored"))
            lText.text = "S";
        else
            lText.text = l;
        if (r.Equals("stored"))
            rText.text = "S";
        else
            rText.text = r;
    }

    private IEnumerator ReverseAnimation(SortingElement s1, SortingElement s2,int step) {
        float prevTime = Time.time;
        float duration = .5f;
        float elapsed = 0f;
        float percent;
        Vector3 lPath = s1.transform.position - leftOrigin;
        Vector3 rPath = s2.transform.position - rightOrigin;
        Left.SetActive(true);
        Right.SetActive(true);
        while (elapsed < duration) {
            percent = elapsed / duration;
            Left.transform.position = leftOrigin + lPath * percent;
            Right.transform.position = rightOrigin + rPath * percent;
            float time = Time.time;
            elapsed += time - prevTime;
            prevTime = time;
            yield return null;
        }
        Left.transform.position = leftOrigin;
        Right.transform.position = rightOrigin;
        Left.SetActive(false);
        Right.SetActive(false);
        LoadPrev(step);
        routine = null;
        EventManager.ActionCompleted(true);
    }

    private IEnumerator CompareAnimation(SortingElement s1, SortingElement s2, bool reverse = false) {
        float prevTime = Time.time;
        float duration = .5f;
        float elapsed = 0f;
        float percent;
        Vector3 lStart = s1.transform.position;
        Vector3 lPath = Left.transform.position - lStart;
        Vector3 rStart = s2.transform.position;
        Vector3 rPath = Right.transform.position - rStart;
        Left.SetActive(true);
        Right.SetActive(true);
        while(elapsed < duration) {
            percent = elapsed / duration;
            Left.transform.position = lStart + lPath * percent;
            Right.transform.position = rStart + rPath * percent;
            float time = Time.time;
            elapsed += time - prevTime;
            prevTime = time;
            yield return null;
        }
        Left.transform.position = lStart + lPath;
        Right.transform.position = rStart + rPath;

        SetState(s1, s2);
        routine = null;
        EventManager.ActionCompleted();
    }
}
