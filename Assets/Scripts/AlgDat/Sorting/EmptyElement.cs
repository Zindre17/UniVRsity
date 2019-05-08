using System.Collections;
using UnityEngine;

public class EmptyElement : Selectable
{
    private Color color;
    private void Awake() {
        Index = -2;
        color = rend.material.color; 
    }
    internal override void UpdateColor() {
        if (cm == null)
            cm = ColorManager.instance;
        Color c = selected ? cm.selected : color;
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
