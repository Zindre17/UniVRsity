using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortingToggles : MonoBehaviour
{
    public UIButton bubble, insertion, quick;

    public UIButton demo, copyTo;

    public void CopyTo(bool a) {
        if (a)
            copyTo.Select();
        else
            copyTo.Deselect();
    }

    public void Demo(bool a) {
        if (a)
            demo.Select();
        else
            demo.Deselect();
    }

    public void Bubble() {
        AlgToggle(true, false, false);
    }

    public void Insertion() {
        AlgToggle(false, true, false);
    }
    
    public void Quick() {
        AlgToggle(false, false, true);
    }

    private void AlgToggle(bool b, bool i , bool q) {
        if (b)
            bubble.Select();
        else
            bubble.Deselect();

        if (i)
            insertion.Select();
        else
            insertion.Deselect();

        if (q)
            quick.Select();
        else
            quick.Deselect();
    }
}
