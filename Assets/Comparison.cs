using UnityEngine;
using TMPro;
using System.Collections;

public class Comparison : MonoBehaviour
{

    public GameObject Left;
    public GameObject Right;
    public TextMeshPro lText;
    public TextMeshPro rText;
    public TextMeshPro comparison;

    private Coroutine routine;

    public void Clear() {
        if (routine != null) {
            StopCoroutine(routine);
            routine = null;
        }
        Left.SetActive(false);
        Right.SetActive(false);
        lText.text = "";
        rText.text = "";
        comparison.text = "";
    }
    
    public void Compare(SortingElement s1, SortingElement s2) {
        
        routine = StartCoroutine(CompareAnimation(s1, s2));
    }

    private IEnumerator CompareAnimation(SortingElement s1, SortingElement s2) {
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
        routine = null;
        EventManager.ActionCompleted();
    }
}
