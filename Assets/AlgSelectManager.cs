using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlgSelectManager : MonoBehaviour
{
    public UIButton bubble, insertion, quick, merge;

    public void UpdateStates(bool canChange)
    {
        bubble.Active = canChange;
        insertion.Active = canChange;
        quick.Active = canChange;
        merge.Active = canChange;
    }

    public void Bubble()
    {
        UpdateToggle(b: true);
    }
    public void Insertion()
    {
        UpdateToggle(i: true);
    }
    public void Quick()
    {
        UpdateToggle(q: true);
    }
    public void Merge()
    {
        UpdateToggle(m: true);
    }

    public void UpdateToggle(bool b = false, bool i = false, bool q = false, bool m = false)
    {
        bubble.Toggled = b;
        bubble.Active = !b;
        insertion.Toggled = i;
        insertion.Active = !i;
        quick.Toggled = q;
        quick.Active = !q;
        merge.Toggled = m;
        merge.Active = !m;
    }
}
