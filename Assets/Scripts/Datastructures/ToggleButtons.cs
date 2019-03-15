using System.Collections.Generic;
using UnityEngine;

public class ToggleButtons: MonoBehaviour
{
    private UIButton selected;

    public void Select(UIButton b) {
        if (selected != null)
            selected.Deselect();
        b.Select();
        selected = b;
    }
}
