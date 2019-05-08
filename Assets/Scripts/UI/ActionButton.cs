using System;
using System.Collections;
using UnityEngine;

public class ActionButton : UIButton {

    public bool multistep;
    private TMPro.TextMeshPro label = null;
    private bool inProgress = false;

    public bool InProgress {
        get { return inProgress; }
        set {
            if(value != inProgress) {
                inProgress = value;
                UpdateColor();
            }
        }
    }

    public void SetLabel(string text)
    {
        if (label == null)
            label = GetComponentInChildren<TMPro.TextMeshPro>();
        label.text = text;
    }

    public override bool Press(bool demo = false, bool pause = false, Action function = null) {
        if (base.Press(demo, pause, function)) {
            if (multistep)
                InProgress = true;
            return true;
        }   
        return false;
    }


    public void Hint() {
        StartCoroutine(HintAnimation());
    }

    protected override void UpdateColor() {
        if (inProgress)
            rend.material.color = colorManager.inProgress;
        else
            base.UpdateColor();
    }

    private IEnumerator HintAnimation() {
        UpdateColor();
        Color def = rend.material.color;
        Color hint = colorManager.hint;
        yield return new WaitForSeconds(.1f);
        rend.material.color = hint;
        yield return new WaitForSeconds(.3f);
        rend.material.color = def;
        yield return new WaitForSeconds(.3f);
        rend.material.color = hint;
        yield return new WaitForSeconds(.3f);
        rend.material.color = def;
    }
}
