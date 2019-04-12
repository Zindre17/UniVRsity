using System.Collections;
using UnityEngine;

public class EmptyElement : Selectable
{
    private void Awake() {
        Index = -2;
    }
    internal override void UpdateColor() {
        if (cm == null)
            cm = ColorManager.instance;
        Color c = selected ? cm.selected : cm.box;
        c = new Color(c.r, c.g, c.b, selected? .5f:.3f);
        rend.material.color = c;
    }


    internal override void SetActive(bool a) {
    }

    internal override void SetSelected(bool s) {
        if (cm == null)
            cm = ColorManager.instance;
        UpdateColor();
    }
}
